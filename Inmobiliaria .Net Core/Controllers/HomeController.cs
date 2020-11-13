using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepositorioPropietario propietarios;
        private readonly IConfiguration config;

        public HomeController(IRepositorioPropietario propietarios, IConfiguration config)
        {
            this.propietarios = propietarios;
            this.config = config;
        }

        public IActionResult Index()
        {
            ViewBag.Titulo = "Página de Inicio";
            List<string> clientes = propietarios.ObtenerTodos().Select(x => x.Nombre + " " + x.Apellido).ToList();
            return View(clientes);
        }

        [Authorize]
        public ActionResult Seguro()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            return View(claims);
        }

        [Authorize(Policy = "Administrador")]
        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult Restringido()
        {
            return View();
        }

        public IActionResult Fecha(int anio, int mes, int dia)
        {
            DateTime dt = new DateTime(anio, mes, dia);
            ViewBag.Fecha = dt;
            return View();
        }

        public IActionResult Ruta(string valor)
        {
            ViewBag.Valor = valor;
            return View();
        }

        public IActionResult Chat()
        {
            return View();
        }

        public IActionResult ChatSeguro()
        {
            return View();
        }
    }
}