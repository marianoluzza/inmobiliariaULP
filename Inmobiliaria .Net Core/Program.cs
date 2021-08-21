using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Inmobiliaria_.Net_Core
{
	public class Program
	{
		public static void Main(string[] args)
		{
           CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureLogging(logging =>
				{
					logging.ClearProviders();//limpia los proveedores x defecto de log (consola+depuración)
					logging.AddConsole();//agrega log de consola
					//logging.AddConfigur(new LoggerConfiguration().WriteTo.File("serilog.txt").CreateLogger())
				})
				.UseStartup<Startup>();
	}
}
