using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;

namespace Inmobiliaria_.Net_Core.Controllers
{
	public class PropietariosController : Controller
	{
		// Sin inyección de dependencias (crear dentro del ctor)
		//private readonly RepositorioPropietario repositorio;

		// Con inyección de dependencias (pedir en el ctor como parámetro)
		private readonly IRepositorioPropietario repositorio;
		private readonly IConfiguration config;
		private readonly IServicioClaves claves;
		private readonly ILogger<PropietariosController> logger;

		public PropietariosController(IRepositorioPropietario repo, IConfiguration config, IServicioClaves claves, ILogger<PropietariosController> logger)
		{
			// Sin inyección de dependencias y sin usar el config (quitar el parámetro repo del ctor)
			//this.repositorio = new RepositorioPropietario();
			// Sin inyección de dependencias y pasando el config (quitar el parámetro repo del ctor)
			//this.repositorio = new RepositorioPropietario(config);
			// Con inyección de dependencias
			this.repositorio = repo;
			this.config = config;
			this.claves = claves;
			this.logger = logger;
		}

		// GET: Propietarios
		[Route("[controller]/Index")]
		public ActionResult Index(int pagina=1)
		{
			try
			{
				var tamaño = 5;
				var lista = repositorio.ObtenerLista(Math.Max(pagina, 1), tamaño);
				ViewBag.Tamaño = tamaño;
				ViewBag.Pagina = pagina;
				var total = repositorio.ObtenerCantidad();
				ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
				ViewBag.Id = TempData["Id"];
				// TempData es para pasar datos entre acciones
				// ViewBag/Data es para pasar datos del controlador a la vista
				// Si viene alguno valor por el tempdata, lo paso al viewdata/viewbag
				if (TempData.ContainsKey("Mensaje"))
					ViewBag.Mensaje = TempData["Mensaje"];
				return View(lista);
			}
			catch (Exception ex)
			{// Poner breakpoints para detectar errores
				logger.LogError(ex, "Error en Index");
				throw;
			}
		}

		// GET: Propietarios/Details/5
		public ActionResult Details(int id)
		{
			try
			{
				var entidad = repositorio.ObtenerPorId(id);
				return View(entidad);
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				logger.LogError(ex, "Error en Details");
				throw;
			}
		}

		// GET: Propietarios/Obtener/5
		public IActionResult Obtener(int id)
		{
			try
			{
				var res = repositorio.ObtenerPorId(id);
				// Se devuelve PropietarioView y no la entidad, para que no viaje la clave
				return Ok(res == null ? null : new PropietarioView(res));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET: Propietarios/Busqueda
		public IActionResult Busqueda()
		{
			try
			{
				return View();
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				logger.LogError(ex, "Error en Busqueda");
				throw;
			}
		}

		// GET: Propietarios/Buscar/5
		[Route("[controller]/Buscar/{q}", Name = "Buscar")]
		public IActionResult Buscar(string q)
		{
			try
			{
				var res = repositorio.BuscarPorNombre(q);
				return Json(new { Datos = res.Select(x => new PropietarioView(x)) });
			}
			catch (Exception ex)
			{
				return Json(new { Error = ex.Message });
			}
		}

		// GET: Propietarios/Create
		public ActionResult Create()
		{
			try
			{
				return View();
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				logger.LogError(ex, "Error en Create");
				throw;
			}
		}

		// POST: Propietarios/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Propietario propietario)
		{
			try
			{
				if (ModelState.IsValid)// Pregunta si el modelo es válido
				{
					// Reemplazo de clave plana por clave con hash
					propietario.Clave = claves.Hashear(propietario.Clave);
					repositorio.Alta(propietario);
					TempData["Id"] = propietario.IdPropietario;
					return RedirectToAction(nameof(Index));
				}
				else
					return View(propietario);
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				logger.LogError(ex, "Error en Create");
				throw;
			}
		}

		// GET: Propietarios/Edit/5
		public ActionResult Edit(int id)
		{
			try
			{
				var entidad = repositorio.ObtenerPorId(id);
				return View(entidad);//pasa el modelo a la vista
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				logger.LogError(ex, "Error en Edit");
				throw;
			}
		}

		// POST: Propietarios/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		//public ActionResult Edit(int id, IFormCollection collection)
		public ActionResult Edit(int id, Propietario entidad)
		{
			// Si en lugar de IFormCollection ponemos Propietario, el enlace de datos lo hace el sistema
			try
			{
				var p = repositorio.ObtenerPorId(id);
				if (p == null)
					return NotFound();
				// En caso de ser necesario usar: 
				//
				//Convert.ToInt32(collection["CAMPO"]);
				//Convert.ToDecimal(collection["CAMPO"]);
				//Convert.ToDateTime(collection["CAMPO"]);
				//int.Parse(collection["CAMPO"]);
				//decimal.Parse(collection["CAMPO"]);
				//DateTime.Parse(collection["CAMPO"]);
				////////////////////////////////////////
				p.Nombre = entidad.Nombre;
				p.Apellido = entidad.Apellido;
				p.Dni = entidad.Dni;
				p.Email = entidad.Email;
				p.Telefono = entidad.Telefono;
				repositorio.Modificacion(p);
				TempData["Mensaje"] = "Datos guardados correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				logger.LogError(ex, "Error en Edit");
				throw;
			}
		}

		// POST: Propietarios/Guardar/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Guardar(int id, Propietario entidad)
		{
			try
			{
				// Al editar no se cambia la clave (ni se recibe: Obtener ya no la devuelve)
				if(id != 0)
					ModelState.Remove(nameof(Propietario.Clave));
				if(!ModelState.IsValid)
					return BadRequest(ModelState);
				if(id == 0)
				{
					entidad.Clave = claves.Hashear(entidad.Clave);
					id = repositorio.Alta(entidad);
				}
				else
				{
					var p = repositorio.ObtenerPorId(id);
					if (p == null)
						return NotFound();
					p.Nombre = entidad.Nombre;
					p.Apellido = entidad.Apellido;
					p.Dni = entidad.Dni;
					p.Email = entidad.Email;
					p.Telefono = entidad.Telefono;
					repositorio.Modificacion(p);
				}
				var res = repositorio.ObtenerPorId(id);
				return Ok(res == null ? null : new PropietarioView(res));
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				return BadRequest(ex.Message);
			}
		}

		// POST: Propietarios/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CambiarPass(int id, CambioClaveView cambio)
		{
			Propietario? propietario = null;
			try
			{
				// recuperar propietario original
				propietario = repositorio.ObtenerPorId(id);
				// verificar clave antigüa (si estaba en el formato viejo, igual se reemplaza abajo)
				if (propietario == null || claves.Verificar(propietario.Clave, cambio.ClaveVieja ?? "") == ResultadoVerificacion.Incorrecta)
				{
					TempData["Error"] = "Clave incorrecta";
					// se rederige porque no hay vista de cambio de pass, está compartida con Edit
					return RedirectToAction("Edit", new { id = id });
				}
				if (ModelState.IsValid)
				{
					propietario.Clave = claves.Hashear(cambio.ClaveNueva);
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
				logger.LogError(ex, "Error en CambiarPass");
				TempData["Error"] = ex.Message;
				TempData["StackTrace"] = ex.StackTrace;
				return RedirectToAction("Edit", new { id = id });
			}
		}

		// GET: Propietarios/Delete/5
		public ActionResult Eliminar(int id)
		{
			try
			{
				var entidad = repositorio.ObtenerPorId(id);
				return View(entidad);
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				logger.LogError(ex, "Error en Eliminar");
				throw;
			}
		}

		// POST: Propietarios/Delete/5
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
				logger.LogError(ex, "Error en Eliminar");
				throw;
			}
		}
	}
}