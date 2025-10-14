using Inmobiliaria_.Net_Core;
using Inmobiliaria_.Net_Core.Api;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Inmobiliaria_Tests
{
	public class PropietariosControllerTest
	{
		Helper helper = new Helper();
		PropietariosController controller;
		public PropietariosControllerTest()
		{
			//https://softchris.github.io/pages/dotnet-moq.html
			var mockEnvironment = new Moq.Mock<IWebHostEnvironment>();
			mockEnvironment
				.Setup(m => m.EnvironmentName)
				.Returns("Development");
			controller = new PropietariosController(helper.DataContext, helper.Config, mockEnvironment.Object);
		}

		[Fact]
		public async Task Get_MiPerfil_PropietarioAutenticado_DevuelvePropietario()
		{
			string email = "mluzza@ulp.edu.ar";
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext() { User = helper.MockLogin(email, "Propietario") }
			};
			var res = await controller.Get();
			var httpRes = res.Result as OkObjectResult;
			Assert.NotNull(httpRes);
			var propietario = httpRes.Value as Propietario;
			Assert.NotNull(propietario);
			Assert.Equal(email, propietario.Email);
			Assert.Equal("Mariano", propietario.Nombre);
		}

		[Fact]
		public async Task Get_MiPerfil_PropietarioNoAutenticado_DevuelveNull()
		{
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal(new ClaimsIdentity()) }
			};
			var res = await controller.Get();
			var propietario = res.Value;
			Assert.Null(propietario);
		}

		[Fact]
		public void Get_MiPerfil_RequiereAutenticacion()
		{
			var tipo = controller.GetType();
			var attrsClase = tipo.GetCustomAttributes(
				typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);
			var auth = attrsClase.Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().FirstOrDefault();
			if (auth == null)
			{
				var metodo = tipo.GetMethod(nameof(controller.Get), System.Type.EmptyTypes);
				if (metodo != null)
				{
					var attrsMetodo = metodo.GetCustomAttributes(
						typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);
					auth = attrsMetodo.Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().FirstOrDefault();
				}
			}
			Assert.NotNull(auth);
			Assert.Equal(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme, auth.AuthenticationSchemes);
			//Assert.Equal("Propietario", auth.Policy);
		}

		[Fact]
		public async Task PerfilProhibidoSinAutenticar()
		{
			// Arrange
			var appServer = BuildWebApplication(helper);
			await appServer.StartAsync();
			var client = appServer.GetTestClient();
			var url = "api/propietarios";
			var codigo = HttpStatusCode.Unauthorized;

			// Act
			var response = await client.GetAsync(url);

			// Assert
			Assert.Equal(codigo, response.StatusCode);
			Assert.Contains("Bearer", response.Headers.WwwAuthenticate.First().ToString(), StringComparison.InvariantCultureIgnoreCase);
		}

		static private WebApplication BuildWebApplication(Helper helper)
		{
			var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(System.Array.Empty<string>());
			builder.Configuration.AddConfiguration(helper.Config);
			builder.Services.AddControllers();
			builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				var secreto = builder.Configuration["TokenAuthentication:SecretKey"];
				if (string.IsNullOrEmpty(secreto))
					throw new System.Exception("Falta configurar TokenAuthentication:Secret");
				options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = builder.Configuration["TokenAuthentication:Issuer"],
					ValidAudience = builder.Configuration["TokenAuthentication:Audience"],
					IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(secreto)),
				};
			});
			builder.Services.AddAuthorization();
			builder.WebHost.UseTestServer();
			var appServer = builder.Build();
			appServer.UseRouting();
			appServer.UseAuthentication();
			appServer.UseAuthorization();
			appServer.MapControllers();
			return appServer;
		}
	}
}
