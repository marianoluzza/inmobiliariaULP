using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Inmobiliaria_.Net_Core.Models
{
	[Table("Inmuebles")]
	public class Inmueble
	{
		[Display(Name = "Nº")]
		public int Id { get; set; }
		//[Required]
		[Display(Name = "Dirección")]
		[Required(ErrorMessage = "La dirección es requerida")]
		public string? Direccion { get; set; }
		[Required]
		public int Ambientes { get; set; }
		[Required]
		public int Superficie { get; set; }
		public decimal Latitud { get; set; }
		public decimal Longitud { get; set; }
		[Display(Name = "Dueño")]
		public int PropietarioId { get; set; }
		[ForeignKey(nameof(PropietarioId))]
    [BindNever]
		public Propietario? Duenio { get; set; }
		public string? Portada { get; set; }
		[NotMapped]//Para EF
		public IFormFile? PortadaFile { get; set; }
		[ForeignKey(nameof(Imagen.InmuebleId))]
		public IList<Imagen> Imagenes { get; set; } = new List<Imagen>();
		[NotMapped]
		public bool Habilitado { get; set; } = true;
	}
	
}
