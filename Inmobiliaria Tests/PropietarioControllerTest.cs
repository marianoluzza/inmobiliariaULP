using Inmobiliaria_.Net_Core;
using Inmobiliaria_.Net_Core.Api;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
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
			int idPropietario = 7;

			// Usar SQLite in-memory para aislar el test y sembrar los datos necesarios
			using var connection = Helper.CreateInMemoryDatabase();
			var options = Helper.CreateOptions(connection);

			// Crear esquema y seed
			using (var seedCtx = new DataContext(options))
			{
				seedCtx.Database.EnsureCreated();
				seedCtx.Propietarios.Add(new Propietario { IdPropietario = idPropietario, Email = email, Nombre = "Mariano", Apellido = "Luzza" });
				seedCtx.SaveChanges();
			}

			// Crear un controller que use el contexto sqlite
			using (var context = new DataContext(options))
			{
				var localController = new PropietariosController(context, helper.Config, mockEnvironment.Object, new Inmobiliaria_.Net_Core.Services.ServicioClaves(helper.Config), Moq.Mock.Of<Microsoft.Extensions.Logging.ILogger<PropietariosController>>());
				localController.ControllerContext = new ControllerContext()
				{
					HttpContext = new DefaultHttpContext() { User = helper.MockLogin(email, "Propietario", idPropietario) }
				};

				var res = await localController.Get();
				var httpRes = res.Result as OkObjectResult;
				Assert.NotNull(httpRes);
				var propietario = httpRes.Value as PropietarioView;
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
				var localController = new PropietariosController(context, helper.Config, mockEnvironment.Object, new Inmobiliaria_.Net_Core.Services.ServicioClaves(helper.Config), Moq.Mock.Of<Microsoft.Extensions.Logging.ILogger<PropietariosController>>());
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
		public async Task Get_MiPerfil_NoDevuelveLaClave()
		{
			using var connection = Helper.CreateInMemoryDatabase();
			var options = Helper.CreateOptions(connection);
			using (var seedCtx = new DataContext(options))
			{
				seedCtx.Database.EnsureCreated();
				seedCtx.Propietarios.Add(new Propietario { IdPropietario = 7, Email = "p@p.com", Nombre = "Mariano", Apellido = "Luzza", Clave = "hash-guardado" });
				seedCtx.SaveChanges();
			}

			using (var context = new DataContext(options))
			{
				var controller = new PropietariosController(context, helper.Config, mockEnvironment.Object, new Inmobiliaria_.Net_Core.Services.ServicioClaves(helper.Config), Moq.Mock.Of<Microsoft.Extensions.Logging.ILogger<PropietariosController>>());
				controller.ControllerContext = new ControllerContext()
				{
					HttpContext = new DefaultHttpContext() { User = helper.MockLogin("p@p.com", "Propietario", 7) }
				};

				var res = await controller.Get();
				var ok = Assert.IsType<OkObjectResult>(res.Result);
				// Lo que viaja al cliente es el objeto serializado a JSON
				var json = System.Text.Json.JsonSerializer.Serialize(ok.Value);
				Assert.Contains("Mariano", json);
				Assert.DoesNotContain("hash-guardado", json);
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
			using (var factory = new WebApplicationFactory<Program>())
			{
				using (var client = factory.CreateClient())
				{
					// Act
					var response = await client.GetAsync("/api/propietarios");

					// Assert
					Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

					Assert.Contains(
						response.Headers.WwwAuthenticate,
						header => string.Equals(
							header.Scheme,
							"Bearer",
							StringComparison.OrdinalIgnoreCase
						)
					);
				}
			}
		}

		[Fact]
		public async Task Login_ClaveConHashViejo_DevuelveTokenYGuardaElHashNuevo()
		{
			// Las claves de la base de ejemplo están con el hash anterior: tienen que seguir
			// andando, y al iniciar sesión se reemplazan por el formato actual.
			var hashViejo = ServicioClavesTest.HashLegado("123");
			using var connection = Helper.CreateInMemoryDatabase();
			var options = Helper.CreateOptions(connection);
			using (var seedCtx = new DataContext(options))
			{
				seedCtx.Database.EnsureCreated();
				seedCtx.Propietarios.Add(new Propietario { IdPropietario = 7, Email = "p@p.com", Nombre = "N", Apellido = "A", Clave = hashViejo });
				seedCtx.SaveChanges();
			}

			var claves = ServicioClavesTest.Crear();
			using (var context = new DataContext(options))
			{
				var controller = new PropietariosController(context, helper.Config, mockEnvironment.Object, claves, Moq.Mock.Of<Microsoft.Extensions.Logging.ILogger<PropietariosController>>());
				var res = await controller.Login(new LoginView { Usuario = "p@p.com", Clave = "123" });
				Assert.IsType<OkObjectResult>(res);
			}

			using (var context = new DataContext(options))
			{
				var guardada = context.Propietarios.Single(x => x.IdPropietario == 7).Clave;
				Assert.NotEqual(hashViejo, guardada);
				Assert.Equal(Inmobiliaria_.Net_Core.Services.ResultadoVerificacion.Correcta, claves.Verificar(guardada, "123"));
			}
		}

		[Fact]
		public async Task Post_GuardaLaClaveHasheada()
		{
			using var connection = Helper.CreateInMemoryDatabase();
			var options = Helper.CreateOptions(connection);
			using (var seedCtx = new DataContext(options))
				seedCtx.Database.EnsureCreated();

			var claves = ServicioClavesTest.Crear();
			using (var context = new DataContext(options))
			{
				var controller = new PropietariosController(context, helper.Config, mockEnvironment.Object, claves, Moq.Mock.Of<Microsoft.Extensions.Logging.ILogger<PropietariosController>>());
				var res = await controller.Post(new Propietario { Email = "nuevo@p.com", Nombre = "N", Apellido = "A", Clave = "123" });
				Assert.IsType<CreatedAtActionResult>(res);
			}

			using (var context = new DataContext(options))
			{
				var guardada = context.Propietarios.Single(x => x.Email == "nuevo@p.com").Clave;
				Assert.NotEqual("123", guardada);
				Assert.Equal(Inmobiliaria_.Net_Core.Services.ResultadoVerificacion.Correcta, claves.Verificar(guardada, "123"));
			}
		}
	}
}
