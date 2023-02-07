using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Controllers
{
	public class PersonasController : Controller
	{
		protected readonly IConfiguration configuration;
		RepositorioPersona repositorio;

		public PersonasController(IConfiguration configuration)
		{
			this.configuration = configuration;
			repositorio = new RepositorioPersona();
		}

		// GET: PersonasController
		public ActionResult Index()
		{
			var lta = repositorio.ObtenerTodas();
			ViewBag.Cantidad = 3;
			var p = new Persona
			{
				Id = -1,
				Nombre = "No existe",
			};
			ViewData["Datos"] = p;
			ViewBag.Otro = "Hola";
			return View(lta);
		}

		// GET: PersonasController/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: PersonasController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: PersonasController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: PersonasController/Edit/5
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: PersonasController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		// GET: PersonasController/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: PersonasController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, IFormCollection collection)
		{
			try
			{
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}
	}
}
