using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Inmobiliaria_.Net_Core.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(new
                {
                    Mensaje = "Éxito",
                    Error = 0,
                    Resultado = new
                    {
                        Clave = "Key",
                        Valor = "Value"
                    },
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromForm]string value, IFormFile file)
        {

        }

        // POST api/<controller>/usuario/5
        [HttpPost("usuario/{id}")]
        public void Post([FromForm] Usuario usuario, int id)
        {

        }

        // POST api/<controller>/usuario/5
        [HttpPost("login")]
        public async Task<IActionResult> Post([FromForm]LoginView login)
        {
            return Ok(login);
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
