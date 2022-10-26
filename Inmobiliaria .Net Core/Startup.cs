using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Hosting;
using Inmobiliaria_.Net_Core.Controllers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Hosting;

namespace Inmobiliaria_.Net_Core
{
	public class Startup
	{
		private readonly IConfiguration configuration;

		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
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
			services.AddAuthorization(options =>
			{
				//options.AddPolicy("Empleado", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador", "Empleado"));
				options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador", "SuperAdministrador"));
			});
			services.AddMvc();
			services.AddSignalR();//añade signalR
			//IUserIdProvider permite cambiar el ClaimType usado para obtener el UserIdentifier en Hub
			services.AddSingleton<IUserIdProvider, UserIdProvider>();
			// SOLO PARA INYECCIÓN DE DEPENDECIAS:
			/*
			Transient objects are always different; a new instance is provided to every controller and every service.
			Scoped objects are the same within a request, but different across different requests.
			Singleton objects are the same for every object and every request.
			*/
			services.AddTransient<IRepositorio<Propietario>, RepositorioPropietario>();
			services.AddTransient<IRepositorioPropietario, RepositorioPropietario>();
			//services.AddTransient<IRepositorio<Inquilino>, RepositorioInquilino>();
			services.AddTransient<IRepositorioInmueble, RepositorioInmueble>();
			services.AddTransient<IRepositorioUsuario, RepositorioUsuario>();
			// SOLO SI USA ENTITY FRAMEWORK:
			services.AddDbContext<DataContext>(
				options => options.UseSqlServer(
					configuration["ConnectionStrings:DefaultConnection"]
				)
			);
			/* PARA MySql - usando Pomelo */
			//services.AddDbContext<DataContext>(
			//	options => options.UseMySql(
			//		configuration["ConnectionStrings:DefaultConnection"],
			//		ServerVersion.AutoDetect(configuration["ConnectionStrings:DefaultConnection"])
			//	)
			//);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			// Todos estos métodos permiten manejar errores 404
			// En lugar de devolver el error, devuelve el código
			//app.UseStatusCodePages();
			// Hace un redirect cuando ocurren errores
			//app.UseStatusCodePagesWithRedirects("/Home/Error/{0}");
			// Hace una reejecución cuando ocurren errores
			//app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

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
			// App en ambiente de desarrollo?
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();//página amarilla de errores
			}
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute("login", "entrar/{**accion}", new { controller = "Usuarios", action = "Login" });
				endpoints.MapControllerRoute("rutaFija", "ruteo/{valor}", new { controller = "Home", action = "Ruta", valor = "defecto" });
				endpoints.MapControllerRoute("fechas", "{controller=Home}/{action=Fecha}/{anio}/{mes}/{dia}");
				endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
				//
				endpoints.MapHub<ChatHub>("/chathub");//para SignalR
				endpoints.MapHub<ChatSeguroHub>("/chatsegurohub");//para SignalR
			});
		}
	}
}
