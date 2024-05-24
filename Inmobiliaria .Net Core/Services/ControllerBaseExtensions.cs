using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Inmobiliaria_.Net_Core.Services
{
	public static class ControllerBaseExtensions
	{
		/// <summary>
		/// Renderiza una vista parcial a string
		/// </summary>
		public static async Task<string> RenderVista<TModel>(this ControllerBase controllerBase, string rutaView, TModel modelo, IRazorViewEngine razorViewEngine, IServiceProvider serviceProvider)
		{
			var viewEngineResult = razorViewEngine.GetView(null, rutaView, false);

			if (!viewEngineResult.Success)
			{
				throw new InvalidOperationException(string.Format("La vista '{0}' no pudo ser ubicada.", rutaView));
			}

			var view = viewEngineResult.View;
			using (var sw = new StringWriter())
			{
				var viewContext = new ViewContext(
					new ActionContext(controllerBase.HttpContext, controllerBase.RouteData, controllerBase.ControllerContext.ActionDescriptor),
					view,
					new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
					{
						Model = modelo
					},
					new TempDataDictionary(controllerBase.HttpContext, serviceProvider.GetRequiredService<ITempDataProvider>()),
					sw,
					new HtmlHelperOptions()
				);

				await view.RenderAsync(viewContext);
				var vista = sw.ToString();
				return vista;
			}
		}

		public static string GenerarUrlCompleta(this ControllerBase controllerBase, string actionName, string controllerName, IWebHostEnvironment environment)
		{
			// Obtiene el esquema (http o https)
			string scheme = controllerBase.Request.Scheme;

			// Obtiene la autoridad (dominio y puerto)
			string dominio = environment.IsDevelopment()? GetLocalIpAddress(): controllerBase.Request.Host.Host;

			// Obtiene el puerto
			var port = controllerBase.Request.Host.Port;

			// Genera la ruta relativa a la acción específica del controlador
			var relativeUrl = controllerBase.Url.Action(actionName, controllerName);

			// Construye la URL completa
			string urlCompleta = $"{scheme}://{dominio}:{port}{relativeUrl}";

			return urlCompleta;
		}

		private static string GetLocalIpAddress()
		{
			string localIp = "";

			// Obtiene todas las direcciones IP del host local
			var host = Dns.GetHostEntry(Dns.GetHostName());

			foreach (var ip in host.AddressList)
			{
				// Selecciona la dirección IPv4 no loopback
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					localIp = ip.ToString();
					break;
				}
			}

			return localIp;
		}
	}
}