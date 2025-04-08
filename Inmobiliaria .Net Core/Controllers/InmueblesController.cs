using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria_.Net_Core.Controllers
{
	[Authorize]
	public class InmueblesController : Controller
	{
		private readonly IRepositorioInmueble repositorio;
		private readonly IRepositorioPropietario repoPropietario;

		public InmueblesController(IRepositorioInmueble repositorio, IRepositorioPropietario repoPropietrio)
		{
			this.repositorio = repositorio;
			this.repoPropietario = repoPropietrio;
		}

		// GET: Inmueble
		public ActionResult Index()
		{
			var lista = repositorio.ObtenerTodos();
			if (TempData.ContainsKey("Id"))
				ViewBag.Id = TempData["Id"];
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			return View(lista);
		}

		public ActionResult PorPropietario(int id)
		{
			var lista = repositorio.ObtenerTodos();//repositorio.ObtenerPorPropietario(id);
			if (TempData.ContainsKey("Id"))
				ViewBag.Id = TempData["Id"];
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			ViewBag.Id = id;
			//ViewBag.Propietario = repoPropietario.
			return View("Index", lista);
		}

		// GET: Inmueble/Details/5
		public ActionResult Ver(int id)
		{
			var entidad = id == 0 ? new Inmueble() : repositorio.ObtenerPorId(id);
			return View(entidad);
		}

		// POST: Inmueble/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Guardar(Inmueble entidad)
		{
			try
			{
				if (entidad.Id == 0)
				{
					repositorio.Alta(entidad);
					TempData["Id"] = entidad.Id;
				}
				else
				{
					repositorio.Modificacion(entidad);
					TempData["Mensaje"] = "Inmueble modificado correctamente";
				}
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
				ViewBag.StackTrate = ex.StackTrace;
				return View(entidad);
			}
		}

		// GET: Inmueble/Imagenes/5
		public ActionResult Imagenes(int id, [FromServices] IRepositorioImagen repoImagen)
		{
			var entidad = repositorio.ObtenerPorId(id);
			entidad.Imagenes = repoImagen.BuscarPorInmueble(id);
			return View(entidad);
		}

		// POST: Inmueble/Portada
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Portada(Imagen entidad, [FromServices] IWebHostEnvironment environment)
		{
			try
			{
				//Recuperar el inmueble y eliminar la imagen anterior
				var inmueble = repositorio.ObtenerPorId(entidad.InmuebleId);
				if (inmueble != null && inmueble.Portada != null)
				{
					string rutaEliminar = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles", Path.GetFileName(inmueble.Portada));
					System.IO.File.Delete(rutaEliminar);
				}
				if (entidad.Archivo != null)
				{
					string wwwPath = environment.WebRootPath;
					string path = Path.Combine(wwwPath, "Uploads");
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}
					path = Path.Combine(path, "Inmuebles");
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}
					//string fileName = Path.GetFileName(entidad.Archivo.FileName);//este nombre se puede repetir
					string fileName = "portada_" + entidad.InmuebleId + Path.GetExtension(entidad.Archivo.FileName);
					string rutaFisicaCompleta = Path.Combine(path, fileName);
					using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
					{
						entidad.Archivo.CopyTo(stream);
					}
					entidad.Url = Path.Combine("/Uploads/Inmuebles", fileName);
				}
				else //sin imagen
				{
					entidad.Url = string.Empty;
				}
				repositorio.ModificarPortada(entidad.InmuebleId, entidad.Url);
				TempData["Mensaje"] = "Portada actualizada correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				TempData["Error"] = ex.Message;
				return RedirectToAction(nameof(Imagenes), new { id = entidad.InmuebleId });
			}
		}

		// GET: Inmueble/Create
		public ActionResult Create()
		{
			try
			{
				ViewBag.Propietarios = repoPropietario.ObtenerTodos();
				//ViewData["Propietarios"] = repoPropietario.ObtenerTodos();
				//ViewData[nameof(Propietario)] = repoPropietario.ObtenerTodos();
				return View();
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		// POST: Inmueble/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Inmueble entidad)
		{
			try
			{
				if (ModelState.IsValid)
				{
					repositorio.Alta(entidad);
					TempData["Id"] = entidad.Id;
					return RedirectToAction(nameof(Index));
				}
				else
				{
					ViewBag.Propietarios = repoPropietario.ObtenerTodos();
					return View(entidad);
				}
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
				ViewBag.StackTrate = ex.StackTrace;
				return View(entidad);
			}
		}

		// GET: Inmueble/Edit/5
		public ActionResult Edit(int id)
		{
			var entidad = repositorio.ObtenerPorId(id);
			ViewBag.Propietarios = repoPropietario.ObtenerTodos();
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			if (TempData.ContainsKey("Error"))
				ViewBag.Error = TempData["Error"];
			return View(entidad);
		}

		// POST: Inmueble/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, Inmueble entidad)
		{
			try
			{
				entidad.Id = id;
				repositorio.Modificacion(entidad);
				TempData["Mensaje"] = "Datos guardados correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ViewBag.Propietarios = repoPropietario.ObtenerTodos();
				ViewBag.Error = ex.Message;
				ViewBag.StackTrate = ex.StackTrace;
				return View(entidad);
			}
		}

		// GET: Inmueble/Eliminar/5
		public ActionResult Eliminar(int id)
		{
			var entidad = repositorio.ObtenerPorId(id);
			if (TempData.ContainsKey("Mensaje"))
				ViewBag.Mensaje = TempData["Mensaje"];
			if (TempData.ContainsKey("Error"))
				ViewBag.Error = TempData["Error"];
			return View(entidad);
		}

		// POST: Inmueble/Eliminar/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Eliminar(int id, Inmueble entidad)
		{
			try
			{
				repositorio.Baja(id);
				TempData["Mensaje"] = "Eliminación realizada correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ViewBag.Error = ex.Message;
				ViewBag.StackTrate = ex.StackTrace;
				return View(entidad);
			}
		}
	}
}