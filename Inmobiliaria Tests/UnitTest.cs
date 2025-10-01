using Castle.Core.Logging;
using Inmobiliaria_.Net_Core.Controllers;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
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
		public void ViewEditPorId()
		{
			//Arrange
			var mockRepo = new Mock<IRepositorioPropietario>();
			var mockLogger = new Mock<ILogger<PropietariosController>>();
			//los métodos que se hagan mock, deben ser virtuales
			mockRepo.Setup(x => x.ObtenerPorId(1)).Returns(new Propietario { IdPropietario = 1});
			var mockConfig = new Mock<IConfiguration>();
			var controlador = new PropietariosController(mockRepo.Object, mockConfig.Object, mockLogger.Object);
			var httpContext = new DefaultHttpContext();
			var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
			controlador.TempData = tempData;

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
	}
}
