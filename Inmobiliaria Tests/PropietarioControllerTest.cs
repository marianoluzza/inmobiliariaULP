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
		Moq.Mock<IWebHostEnvironment> mockEnvironment = new Moq.Mock<IWebHostEnvironment>();
		public PropietariosControllerTest()
		{
			//https://softchris.github.io/pages/dotnet-moq.html
			mockEnvironment
				.Setup(m => m.EnvironmentName)
				.Returns("Development");
		}

		[Fact]
		public async Task Get_MiPerfil_PropietarioAutenticado_DevuelvePropietario()
		{
			string email = "mluzza@ulp.edu.ar";

			// Usar SQLite in-memory para aislar el test y sembrar los datos necesarios
			using var connection = Helper.CreateInMemoryDatabase();
			var options = Helper.CreateOptions(connection);

			// Crear esquema y seed
			using (var seedCtx = new DataContext(options))
			{
				seedCtx.Database.EnsureCreated();
				seedCtx.Propietarios.Add(new Propietario { Email = email, Nombre = "Mariano", Apellido = "Luzza" });
				seedCtx.SaveChanges();
			}

			// Crear un controller que use el contexto sqlite
			using (var context = new DataContext(options))
			{
				var localController = new PropietariosController(context, helper.Config, mockEnvironment.Object);
				localController.ControllerContext = new ControllerContext()
				{
					HttpContext = new DefaultHttpContext() { User = helper.MockLogin(email, "Propietario") }
				};

				var res = await localController.Get();
				var httpRes = res.Result as OkObjectResult;
				Assert.NotNull(httpRes);
				var propietario = httpRes.Value as Propietario;
				Assert.NotNull(propietario);
				Assert.Equal(email, propietario.Email);
				Assert.Equal("Mariano", propietario.Nombre);
				Assert.Equal("Luzza", propietario.Apellido);
			}
		}

		[Fact]
		public async Task Get_MiPerfil_PropietarioNoAutenticado_DevuelveNull()
		{
			string email = "mluzza@ulp.edu.ar";
			// Usar SQLite in-memory para aislar el test y sembrar los datos necesarios
			using var connection = Helper.CreateInMemoryDatabase();
			var options = Helper.CreateOptions(connection);

			// Crear esquema y seed
			using (var seedCtx = new DataContext(options))
			{
				seedCtx.Database.EnsureCreated();
				seedCtx.Propietarios.Add(new Propietario { Email = email, Nombre = "Mariano", Apellido = "Luzza" });
				seedCtx.SaveChanges();
			}

			// Crear un controller que use el contexto sqlite
			using (var context = new DataContext(options))
			{
				var localController = new PropietariosController(context, helper.Config, mockEnvironment.Object);
				localController.ControllerContext = new ControllerContext()
				{
					HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal(new ClaimsIdentity()) }
				};
				var res = await localController.Get();
				var propietario = res.Value;
				Assert.Null(propietario);
			}
		}

		[Fact]
		public void Get_MiPerfil_RequiereAutenticacion()
		{
			var tipo = typeof(PropietariosController);
			var attrsClase = tipo.GetCustomAttributes(
				typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);
			var auth = attrsClase.Cast<Microsoft.AspNetCore.Authorization.AuthorizeAttribute>().FirstOrDefault();
			if (auth == null)
			{
				var metodo = tipo.GetMethod(nameof(PropietariosController.Get), System.Type.EmptyTypes);
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
		public async Task Get_MiPerfil_ProhibidoSinAutenticar()
		{
			// Arrange
			var appServer = helper.BuildWebApplication();
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

	}
}
