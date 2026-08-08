using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Services;
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
				var propietarioId = this.UsuarioId();
				return Ok(contexto.Inmuebles.Where(e => e.PropietarioId == propietarioId));
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
				var propietarioId = this.UsuarioId();
				return Ok(contexto.Inmuebles.Where(e => e.PropietarioId == propietarioId).Single(e => e.Id == id));
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
					// El id sale del token: ya no hace falta ir a la BD a resolverlo por email.
					entidad.PropietarioId = this.UsuarioId();
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
				var propietarioId = this.UsuarioId();
				if (ModelState.IsValid && contexto.Inmuebles.AsNoTracking().FirstOrDefault(e => e.Id == id && e.PropietarioId == propietarioId) != null)
				{
					entidad.Id = id;
					// El dueño lo fija el servidor: si viniera del body, se podría
					// transferir el inmueble a otro propietario desde el cliente.
					entidad.PropietarioId = propietarioId;
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
				var propietarioId = this.UsuarioId();
				var entidad = contexto.Inmuebles.FirstOrDefault(e => e.Id == id && e.PropietarioId == propietarioId);
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
				var propietarioId = this.UsuarioId();
				var entidad = contexto.Inmuebles.FirstOrDefault(e => e.Id == id && e.PropietarioId == propietarioId);
				if (entidad != null)
				{
					//entidad.Habilitado = false;//TODO: implementar la baja lógica
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
