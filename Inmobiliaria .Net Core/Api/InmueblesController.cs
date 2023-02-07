using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inmobiliaria_.Net_Core.Api
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class InmueblesController : Controller
	{
		private readonly DataContext contexto;

		public InmueblesController(DataContext contexto)
		{
			this.contexto = contexto;
		}

		// GET: api/<controller>
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			try
			{
				var usuario = User.Identity.Name;
				return Ok(contexto.Inmuebles.Include(e => e.Duenio).Where(e => e.Duenio.Email == usuario));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET api/<controller>/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			try
			{
				var usuario = User.Identity.Name;
				return Ok(contexto.Inmuebles.Include(e => e.Duenio).Where(e => e.Duenio.Email == usuario).Single(e => e.Id == id));
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// POST api/<controller>
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] Inmueble entidad)
		{
			try
			{
				if (ModelState.IsValid)
				{
					entidad.PropietarioId = contexto.Propietarios.Single(e => e.Email == User.Identity.Name).IdPropietario;
					contexto.Inmuebles.Add(entidad);
					contexto.SaveChanges();
					return CreatedAtAction(nameof(Get), new { id = entidad.Id }, entidad);
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// PUT api/<controller>/5
		[HttpPut("{id}")]
		public async Task<IActionResult> Put(int id, Inmueble entidad)
		{
			try
			{
				if (ModelState.IsValid && contexto.Inmuebles.AsNoTracking().Include(e => e.Duenio).FirstOrDefault(e => e.Id == id && e.Duenio.Email == User.Identity.Name) != null)
				{
					entidad.Id = id;
					contexto.Inmuebles.Update(entidad);
					contexto.SaveChanges();
					return Ok(entidad);
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// DELETE api/<controller>/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var entidad = contexto.Inmuebles.Include(e => e.Duenio).FirstOrDefault(e => e.Id == id && e.Duenio.Email == User.Identity.Name);
				if (entidad != null)
				{
					contexto.Inmuebles.Remove(entidad);
					contexto.SaveChanges();
					return Ok();
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// DELETE api/<controller>/5
		[HttpDelete("BajaLogica/{id}")]
		public async Task<IActionResult> BajaLogica(int id)
		{
			try
			{
				var entidad = contexto.Inmuebles.Include(e => e.Duenio).FirstOrDefault(e => e.Id == id && e.Duenio.Email == User.Identity.Name);
				if (entidad != null)
				{
					entidad.Superficie = -1;//cambiar por estado = 0
					contexto.Inmuebles.Update(entidad);
					contexto.SaveChanges();
					return Ok();
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
