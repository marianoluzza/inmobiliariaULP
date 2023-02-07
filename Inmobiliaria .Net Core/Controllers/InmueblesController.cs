using System;
using System.Collections.Generic;
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
		public ActionResult Details(int id)
		{
			var entidad = repositorio.ObtenerPorId(id);
			return View(entidad);
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