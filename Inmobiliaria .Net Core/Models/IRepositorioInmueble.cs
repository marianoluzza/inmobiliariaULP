using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public interface IRepositorioInmueble : IRepositorio<Inmueble>
	{
		IList<Inmueble> BuscarPorPropietario(int idPropietario);
    }
}
