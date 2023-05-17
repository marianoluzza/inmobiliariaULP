using System;
using System.Collections.Generic;
using System.Linq;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Inmobiliaria_.Net_Core.Controllers;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000","https://localhost:5001", "http://*:5000", "https://*:5001");
var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>//el sitio web valida con cookie
	{
		options.LoginPath = "/Usuarios/Login";
		options.LogoutPath = "/Usuarios/Logout";
		options.AccessDeniedPath = "/Home/Restringido";
		//options.ExpireTimeSpan = TimeSpan.FromMinutes(5);//Tiempo de expiración
	})
	.AddJwtBearer(options =>//la api web valida con token
	{
		options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = configuration["TokenAuthentication:Issuer"],
			ValidAudience = configuration["TokenAuthentication:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(
				configuration["TokenAuthentication:SecretKey"])),
		};
		// opción extra para usar el token en el hub y otras peticiones sin encabezado (enlaces, src de img, etc.)
		options.Events = new JwtBearerEvents
		{
			OnMessageReceived = context =>
			{
				// Leer el token desde el query string
				var accessToken = context.Request.Query["access_token"];
				// Si el request es para el Hub u otra ruta seleccionada...
				var path = context.HttpContext.Request.Path;
				if (!string.IsNullOrEmpty(accessToken) &&
					(path.StartsWithSegments("/chatsegurohub") ||
					path.StartsWithSegments("/api/propietarios/reset") ||
					path.StartsWithSegments("/api/propietarios/token")))
				{//reemplazar las urls por las necesarias ruta ⬆
					context.Token = accessToken;
				}
				return Task.CompletedTask;
			}
		};
	});

builder.Services.AddAuthorization(options =>
{
	//options.AddPolicy("Empleado", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador", "Empleado"));
	options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador", "SuperAdministrador"));
});
builder.Services.AddMvc();
builder.Services.AddSignalR();//añade signalR
//IUserIdProvider permite cambiar el ClaimType usado para obtener el UserIdentifier en Hub
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
// SOLO PARA INYECCIÓN DE DEPENDECIAS:
/*
Transient objects are always different; a new instance is provided to every controller and every service.
Scoped objects are the same within a request, but different across different requests.
Singleton objects are the same for every object and every request.
*/
builder.Services.AddTransient<IRepositorio<Propietario>, RepositorioPropietario>();
builder.Services.AddTransient<IRepositorioPropietario, RepositorioPropietario>();
//builder.Services.AddTransient<IRepositorioPropietario, RepositorioPropietarioMySql>();
//builder.Services.AddTransient<IRepositorio<Inquilino>, RepositorioInquilino>();
builder.Services.AddTransient<IRepositorioInmueble, RepositorioInmueble>();
builder.Services.AddTransient<IRepositorioUsuario, RepositorioUsuario>();
// SOLO SI USA ENTITY FRAMEWORK:
builder.Services.AddDbContext<DataContext>(
	options => options.UseSqlServer(
		configuration["ConnectionStrings:DefaultConnection"]
	)
);
/* PARA MySql - usando Pomelo */
//builder.Services.AddDbContext<DataContext>(
//	options => options.UseMySql(
//		configuration["ConnectionStrings:DefaultConnection"],
//		ServerVersion.AutoDetect(configuration["ConnectionStrings:DefaultConnection"])
//	)
//);
//SQLite: https://www.nuget.org/packages/Microsoft.Data.Sqlite + https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/
var app = builder.Build();
// Estos métodos permiten manejar errores 404:
// En lugar de devolver el error, devuelve el código
//app.UseStatusCodePages();
// Hace un redirect cuando ocurren errores
//app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");
// Hace una reejecución cuando ocurren errores
//app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
app.UseHttpsRedirection();//comentar para trabajar con http solo
// Habilitar CORS
app.UseCors(x => x
	.AllowAnyOrigin()
	.AllowAnyMethod()
	.AllowAnyHeader());
// Uso de archivos estáticos (*.html, *.css, *.js, etc.)
app.UseStaticFiles();
app.UseRouting();
// Permitir cookies
app.UseCookiePolicy(new CookiePolicyOptions
{
	MinimumSameSitePolicy = SameSiteMode.None,
});
// Habilitar autenticación
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute("login", "entrar/{**accion}", new { controller = "Usuarios", action = "Login" });
app.MapControllerRoute("rutaFija", "ruteo/{valor}", new { controller = "Home", action = "Ruta", valor = "defecto" });
app.MapControllerRoute("fechas", "{controller=Home}/{action=Fecha}/{anio}/{mes}/{dia}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
//
app.MapHub<ChatHub>("/chathub");//para SignalR
app.MapHub<ChatSeguroHub>("/chatsegurohub");//para SignalR
app.Run();
