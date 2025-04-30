using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	[NotMapped]
	public class Imagen
	{
		public int Id { get; set; }
		public int InmuebleId { get; set; }
		public string Url { get; set; } = "";
		public IFormFile? Archivo { get; set; } = null;
	}
}
