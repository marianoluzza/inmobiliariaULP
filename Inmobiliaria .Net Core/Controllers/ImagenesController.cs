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
	public class ImagenesController : Controller
	{
		private readonly IRepositorioImagen repositorio;
		//Sólo estas extensiones se aceptan al subir imágenes
		private static readonly string[] extensionesPermitidas = { ".jpg", ".jpeg", ".png" };

		public ImagenesController(IRepositorioImagen repositorio)
		{
			this.repositorio = repositorio;
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Alta(int id, List<IFormFile> imagenes, [FromServices] IWebHostEnvironment environment)
		{
			if (imagenes == null || imagenes.Count == 0)
				return BadRequest("No se recibieron archivos.");
			//Validar TODAS las extensiones antes de escribir ningún archivo, para no dejar la subida a medias
			if (imagenes.Any(f => !extensionesPermitidas.Contains(Path.GetExtension(f.FileName).ToLowerInvariant())))
				return BadRequest("Las extensiones permitidas son .jpg, .png y .jpeg");
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
			path = Path.Combine(path, id.ToString());
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			foreach (var file in imagenes)
			{
				if (file.Length > 0)
				{
					var extension = Path.GetExtension(file.FileName);
					var nombreArchivo = $"{Guid.NewGuid()}{extension}";
					var rutaArchivo = Path.Combine(path, nombreArchivo);

					using (var stream = new FileStream(rutaArchivo, FileMode.Create))
					{
						await file.CopyToAsync(stream);
					}
					Imagen imagen = new Imagen
					{
						InmuebleId = id,
						Url = $"/Uploads/Inmuebles/{id}/{nombreArchivo}",
					};
					repositorio.Alta(imagen);
				}
			}
			return Ok(repositorio.BuscarPorInmueble(id));
		}

		// POST: Inmueble/Eliminar/5
		[HttpPost]
		[Authorize(Policy = "Administrador")]
		public ActionResult Eliminar(int id)
		{
			try
			{
				//TODO: Eliminar el archivo físico
				var entidad = repositorio.ObtenerPorId(id);
				repositorio.Baja(id);
				return Ok(repositorio.BuscarPorInmueble(entidad.InmuebleId));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}