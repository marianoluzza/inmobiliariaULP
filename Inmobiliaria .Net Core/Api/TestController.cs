using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Inmobiliaria_.Net_Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria_.Net_Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inmobiliaria_.Net_Core.Api
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestController : ControllerBase
	{
		private readonly DataContext Contexto;

		public TestController(DataContext dataContext)
		{
			this.Contexto = dataContext;
		}
		// GET: api/<controller>
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			try
			{
				//throw new Exception("Ocurrió algo");
				return Ok(new
				{
					Mensaje = "Éxito",
					Error = 0,
					Resultado = new
					{
						Clave = "Key",
						Valor = new Random().Next(0, 10000)
					},
				});
			}
			catch (Exception ex)
			{
				return BadRequest(new { Mensaje = ex.Message, Error = 1 });
			}
		}

		// GET api/<controller>/5
		[HttpGet("{id}")]
		public IActionResult Get(int id)
		{
			return Ok(Contexto.Propietarios.Find(id));
		}

		// GET api/<controller>/code/5
		[HttpGet("code/{code}")]
		public IActionResult Code(int code)
		{
			return StatusCode(code, "Hola");
		}

		// GET api/<controller>/usuarios/5
		[HttpGet("usuarios/{id=0}")]
		public IActionResult GetUsers(int id)
		{
			return Ok(Contexto.Usuarios.ToList());
		}

		// GET api/<controller>/5
		[HttpGet("emails/{id=0}")]
		public IActionResult Emails(int id)
		{
			if (id > 0)
				return Ok(Contexto.Propietarios.Where(x => x.IdPropietario == id).Select(x => x.Email).SingleOrDefault());
			else
				return Ok(Contexto.Propietarios.Select(x => x.Email).ToList());
		}

		// GET api/<controller>/5
		[HttpGet("anonimo/{id}")]
		public IActionResult GetAnonimo(int id)
		{
			return id > 0 ?
				Ok(Contexto.Propietarios.Where(x => x.IdPropietario == id)
				.Select(x => new { Id = x.IdPropietario, x.Email }).Single()) :
				Ok(Contexto.Propietarios.Select(x => new { Id = x.IdPropietario, x.Email }).ToList());
		}

		// POST api/<controller>
		[HttpPost]
		public Persona Post([FromBody] Persona value)
		{
			Contexto.Personas.Add(value);
			Contexto.SaveChanges();
			return value;
		}

		// POST api/<controller>/usuario/5
		[HttpPost("usuario/{id}")]
		public Usuario Post([FromForm] Usuario usuario, int id)
		{
			usuario.Id = id;
			return usuario;
		}

		// POST api/<controller>/usuario/5
		[HttpPost("login")]
		public async Task<IActionResult> Post([FromForm] LoginView login)
		{
			return Ok(login);
		}

		// PUT api/<controller>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<controller>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}

		[HttpGet("qr/{code}/{modo=0}")]
		public IActionResult Qr(string code, int modo)
		{
			QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
			QRCoder.QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCoder.QRCodeGenerator.ECCLevel.Q);
			if (modo == 0) //png
			{
				var qrCode = new QRCoder.PngByteQRCode(qrCodeData);
				byte[] qrCodeImage = qrCode.GetGraphic(20);
				return this.File(qrCodeImage, "image/png");
			}
			else //svg
			{
				var qrCode = new QRCoder.SvgQRCode(qrCodeData);
				string qrCodeImage = qrCode.GetGraphic(20);
				return Content(qrCodeImage, "image/svg+xml");
			}
		}

		[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[HttpGet("vista")]
		public async Task<IActionResult> Vista([FromServices]IRazorViewEngine razorViewEngine, [FromServices]IServiceProvider serviceProvider)
		{
			var model = User.Claims;// El modelo para la vista
			var viewPath = "/Views/Home/Seguro.cshtml"; // Ruta de la vista
			var vista = await this.RenderVista<IEnumerable<Claim>>(viewPath, model, razorViewEngine, serviceProvider);
			return Content(vista, "text/html");
		}

		// GET api/<controller>/personas
		[HttpGet("personas")]
		public async Task<IActionResult> Personas()
		{
			//return Ok(Contexto.Personas.Include(x => x.Pasatiempos).ThenInclude(x => x.Pasatiempo).Select(x => new PersonaView(x)));
			var lta = await Contexto.Personas.Include(x => x.Pasatiempos).ThenInclude(x => x.Pasatiempo).ToListAsync();
			return Ok(lta.Select(x => new PersonaView(x)));
		}

		// DELETE api/<controller>/5
		[HttpGet("pasatiempos")]
		public async Task<IActionResult> Pasatiempos()
		{
			return Ok(Contexto.PersonaPasatiempos.Include(x => x.Pasatiempo).Where(x => x.PersonaId == 10).Select(x => x.Pasatiempo));
		}

		// DELETE api/<controller>/5
		[HttpGet("dependientes")]
		public async Task<IActionResult> Dependientes()
		{
			Persona p10 = new Persona
			{
				Id = 10,
				Pasatiempos = new List<PersonaPasatiempo>
				{
					new PersonaPasatiempo {
						Id = 10,
					},
					new PersonaPasatiempo {
						Id = 0,
						PersonaId = 10,
						PasatiempoId = 11,
					},
				},
			};
			Contexto.Personas.Attach(p10);
			await Contexto.SaveChangesAsync();
			return Ok(1);
		}
	}
}
