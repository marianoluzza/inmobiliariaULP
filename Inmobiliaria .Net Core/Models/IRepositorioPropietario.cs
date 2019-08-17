using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public interface IRepositorioPropietario
	{
		int Alta(Propietario p);
		int Baja(int id);
		int Modificacion(Propietario p);

		IList<Propietario> ObtenerTodos();
		Propietario ObtenerPorId(int id);
	}
}
