using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
    public class CambioClaveView
    {
        [DataType(DataType.Password)]
        public string ClaveVieja { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(50, ErrorMessage = "La clave debe tener entre 3 y 50 caracteres", MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string ClaveNueva { get; set; }

        [Required(ErrorMessage = "Debe repetir la contraseña nueva")]
        [StringLength(50, ErrorMessage = "La clave debe tener entre 3 y 50 caracteres", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Compare("ClaveNueva")]
        public string ClaveRepeticion { get; set; }
    }
}
