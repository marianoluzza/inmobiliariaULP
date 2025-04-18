﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public interface IRepositorioImagen : IRepositorio<Imagen>
	{
		IList<Imagen> BuscarPorInmueble(int inmuebleId);
	}
}
