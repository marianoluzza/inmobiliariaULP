using Castle.Core.Logging;
using Inmobiliaria_.Net_Core.Controllers;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Inmobiliaria_Tests
{
	public class UnitTest
	{
		[Fact]
		public void Test1()
		{
			//Arrange
			var x = 1;
			var y = 1;
			//Act
			var res = x + y;
			//Assert
			Assert.Equal(2, res);
		}

		[Fact]
		public void Edit_MuestraFormulario()
		{
			//Arrange
			TempDataDictionary tempData;
			var mockRepo = new Mock<IRepositorioPropietario>();
			//los métodos que se hagan mock, deben ser virtuales
			mockRepo.Setup(x => x.ObtenerPorId(1)).Returns(new Propietario { IdPropietario = 1 });
			//var mockConfig = new Mock<IConfiguration>();
			var controlador = BuildController(mockRepo, out tempData);

			//Act
			var res = controlador.Edit(1) as ViewResult;

			//Assert
			Assert.NotNull(res);
			mockRepo.Verify(x => x.ObtenerPorId(1), Times.Exactly(1), "No se llamó a repo.ObtenerPorId(id)");
			Assert.NotNull(res.Model);
			Assert.IsType<Propietario>(res.Model);
			Propietario p = res.Model as Propietario;
			Assert.Equal(1, p.IdPropietario);
			//Assert.Equal("Edit", res.ViewName);//no se puede porque devuelve View() sin parámetro
		}

		[Fact]
		public void Portada_ReemplazaArchivoYActualizaUrl()
		{
			// Arrange
			var tmp = Path.Combine(Path.GetTempPath(), "inmo_portada_test_" + Guid.NewGuid().ToString("N")); //Ej: "a3d8f6b24e1c4b8a9c2d7e8f9a1b2c3d"
			Directory.CreateDirectory(Path.Combine(tmp, "Uploads", "Inmuebles"));

			var mockRepoInm = new Mock<IRepositorioInmueble>();
			var mockRepoImg = new Mock<IRepositorioImagen>();
			var env = new Mock<IWebHostEnvironment>();
			env.Setup(x => x.WebRootPath).Returns(tmp); //simula wwwroot

			// inmueble existente con portada anterior
			mockRepoInm.Setup(x => x.ObtenerPorId(10)).Returns(new Inmueble { Id = 10, Portada = "/Uploads/Inmuebles/portada_10.jpg" });

			var controlador = new InmueblesController(mockRepoInm.Object, Mock.Of<IRepositorioPropietario>());
			var httpContext = new DefaultHttpContext();
			var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
			controlador.TempData = tempData;

			// Envolvemos en try/finally para garantizar limpieza del directorio temporal
			try
			{
				var archivoFalso = new FormFile(new MemoryStream(new byte[] { 1, 2, 3, 4 }), 0, 4, "Archivo", "nueva.png");
				var img = new Imagen { InmuebleId = 10, Archivo = archivoFalso };

				// Crear archivo anterior
				var oldPath = Path.Combine(tmp, "Uploads", "Inmuebles", "portada_10.jpg");
				File.WriteAllBytes(oldPath, new byte[] { 9, 9, 9 });

				// Act
				var res = controlador.Portada(img, env.Object) as RedirectToActionResult;

				// Assert
				Assert.NotNull(res);
				Assert.Equal(nameof(InmueblesController.Index), res!.ActionName);
				mockRepoInm.Verify(x => x.ModificarPortada(10, It.Is<string>(s => s.EndsWith("portada_10.png"))), Times.Once);

				// el archivo nuevo existe y el viejo fue borrado
				Assert.True(File.Exists(Path.Combine(tmp, "Uploads", "Inmuebles", "portada_10.png")));
				Assert.False(File.Exists(oldPath));

				// Revisar mensaje TempData
				Assert.True(controlador.TempData.ContainsKey("Mensaje"));
				Assert.Equal("Portada actualizada correctamente", controlador.TempData["Mensaje"]);
			}
			finally
			{
				try
				{
					if (Directory.Exists(tmp)) Directory.Delete(tmp, true);
				}
				catch
				{
					// Ignorar errores de limpieza en test
				}
			}
		}

		private static PropietariosController BuildController(Mock<IRepositorioPropietario> mockRepo, out TempDataDictionary tempData)
		{
			var mockLogger = new Mock<ILogger<PropietariosController>>();
			var inMemory = new System.Collections.Generic.Dictionary<string,string> { ["Salt"] = "Salada" };
			var mockConfig = new ConfigurationBuilder().AddInMemoryCollection(inMemory!).Build();

			var controller = new PropietariosController(mockRepo.Object, mockConfig, mockLogger.Object);
			var http = new DefaultHttpContext();
			tempData = new TempDataDictionary(http, Mock.Of<ITempDataProvider>());
			controller.TempData = tempData;
			return controller;
		}
	}
}
