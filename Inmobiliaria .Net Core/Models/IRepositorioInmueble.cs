using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public interface IRepositorioInmueble : IRepositorio<Inmueble>
	{
		int ModificarPortada(int InmuebleId, string ruta);
		IList<Inmueble> BuscarPorPropietario(int idPropietario);
	}
}
