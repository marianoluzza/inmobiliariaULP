using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria_.Net_Core.Controllers
{
    public class PropietariosController : Controller
    {
        private readonly RepositorioPropietario repositorio;
        private readonly IConfiguration config;

        public PropietariosController(IConfiguration config)
        {
            this.repositorio = new RepositorioPropietario(config);
            this.config = config;
        }

        // GET: Propietario
        public ActionResult Index()
        {
			try
            {
                var lista = repositorio.ObtenerTodos();
                ViewBag.Id = TempData["Id"];
                //si viene alguno valor por el tempdata, lo paso al viewdata/viewbag
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                return View(lista);
            }
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
        }

        // GET: Propietario/Details/5
        public ActionResult Details(int id)
        {
			try
            {
                var entidad = repositorio.ObtenerPorId(id);
                return View();
            }
            catch (Exception ex)
            {//poner breakpoints para detectar errores
                throw;
            }
        }

        // GET: Propietario/Busqueda
        public IActionResult Busqueda()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {//poner breakpoints para detectar errores
                throw;
            }
        }

        // GET: Propietario/Buscar/5
        [Route("[controller]/Buscar/{q}", Name = "Buscar")]
        public IActionResult Buscar(string q)
        {
            try
            {
                var res = repositorio.BuscarPorNombre(q);
                return Json(new { Datos = res });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        // GET: Propietario/Create
        public ActionResult Create()
        {
			try
            {
                return View();
            }
            catch (Exception ex)
            {//poner breakpoints para detectar errores
                throw;
            }
        }

        // POST: Propietario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Propietario propietario)
        {
            try
            {
                if (ModelState.IsValid)//pregunta si el modelo es válido
                {
                    //reemplazo de clave plana por clave con hash
                    propietario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: propietario.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    repositorio.Alta(propietario);
                    TempData["Id"] = propietario.IdPropietario;
                    return RedirectToAction(nameof(Index));
                }
                else
                    return View(propietario);
            }
            catch (Exception ex)
            {//poner breakpoints para detectar errores
                throw;
            }
        }

        // GET: Propietario/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                var entidad = repositorio.ObtenerPorId(id);
                return View();
            }
            catch (Exception ex)
            {//poner breakpoints para detectar errores
                throw;
            }
        }

        // POST: Propietario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            //si en lugar de IFormCollection ponemos Propietario, el enlace de datos lo hace el sistema
            Propietario p = null;
            try
            {
                p = repositorio.ObtenerPorId(id);
                //en caso de ser necesario usar: 
                //
                //Convert.ToInt32(collection["CAMPO"]);
                //Convert.ToDecimal(collection["CAMPO"]);
                //Convert.ToDateTime(collection["CAMPO"]);
                //int.Parse(collection["CAMPO"]);
                //decimal.Parse(collection["CAMPO"]);
                //DateTime.Parse(collection["CAMPO"]);
                ////////////////////////////////////////
                p.Nombre = collection["Nombre"];
                p.Apellido = collection["Apellido"];
                p.Dni = collection["Dni"];
                p.Email = collection["Email"];
                p.Telefono = collection["Telefono"];
                repositorio.Modificacion(p);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {//poner breakpoints para detectar errores
                throw;
            }
        }

        // POST: Propietario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarPass(int id, CambioClaveView cambio)
        {
            Propietario propietario = null;
            try
            {
                //recuperar propietario original
                propietario = repositorio.ObtenerPorId(id);
                // verificar clave antigüa
                var pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: cambio.ClaveVieja ?? "",
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                if (propietario.Clave != pass)
                {
                    TempData["Error"] = "Clave incorrecta";
                    //se rederige porque no hay vista de cambio de pass, está compartida con Edit
                    return RedirectToAction("Edit", new { id = id });
                }
                if (ModelState.IsValid)
                {
                    propietario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: cambio.ClaveNueva,
                        salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    repositorio.Modificacion(propietario);
                    TempData["Mensaje"] = "Contraseña actualizada correctamente";
                    return RedirectToAction(nameof(Index));
                }
                else//estado inválido
                {//pasaje de los errores del modelstate a un string en tempData
                    foreach (ModelStateEntry modelState in ViewData.ModelState.Values)
                    {
                        foreach (ModelError error in modelState.Errors)
                        {
                            TempData["Error"] += error.ErrorMessage + "\n";
                        }
                    }
                    return RedirectToAction("Edit", new { id = id });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                TempData["StackTrace"] = ex.StackTrace;
                return RedirectToAction("Edit", new { id = id });
            }
        }

        // GET: Propietario/Delete/5
        public ActionResult Eliminar(int id)
        {
			try
			{
                var entidad = repositorio.ObtenerPorId(id);
                return View(entidad);
            }
            catch (Exception ex)
            {//poner breakpoints para detectar errores
                throw;
            }
        }

        // POST: Propietario/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, Propietario entidad)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {//poner breakpoints para detectar errores
                throw;
            }
        }
    }
}