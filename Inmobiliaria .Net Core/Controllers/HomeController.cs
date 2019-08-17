using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
			ViewBag.Titulo = "Página de Inicio";
			List<string> clientes = new List<string>();
			clientes.Add("Juan");
			clientes.Add("José");
			clientes.Add("Héctor");
			clientes.Add("Hernán");
			return View(clientes);
        }
    }
}