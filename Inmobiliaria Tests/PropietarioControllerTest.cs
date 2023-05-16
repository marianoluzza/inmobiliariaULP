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
		public void MiPerfil()
		{
			string email = "mluzza@ulp.edu.ar";
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext() { User = helper.MockLogin(email, "Propietario") }
			};
			var res = controller.Get().Result.Value;
			Assert.Equal(email, res.Email);
			Assert.Equal("Mariano", res.Nombre);
		}

		[Fact]
		public void PerfilAnonimoInexistente()
		{
			controller.ControllerContext = new ControllerContext()
			{
				HttpContext = new DefaultHttpContext() { User = new ClaimsPrincipal(new ClaimsIdentity()) }
			};
			var res = controller.Get().Result.Value;
			Assert.Null(res);
		}

		[Fact]
		public void ControladorRequiereAutenticacion()
		{
			var atributos = controller.GetType().GetCustomAttributes(
				typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);
			var auth = atributos[0] as Microsoft.AspNetCore.Authorization.AuthorizeAttribute;
			Assert.NotNull(auth);
			Assert.Equal(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme, auth.AuthenticationSchemes);
			//Assert.Equal("Propietario", auth.Policy);
		}

		[Fact]
		public async Task PerfilProhibidoSinAutenticar()
		{
			// Arrange
			var server = new TestServer(new WebHostBuilder()
				.UseConfiguration(helper.Config)
				.UseStartup<Startup>()
			);
			var client = server.CreateClient();
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
