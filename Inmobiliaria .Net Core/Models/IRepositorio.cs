﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public interface IRepositorio<T>
	{
		int Alta(T p);
		int Baja(int id);
		int Modificacion(T p);

		IList<T> ObtenerTodos();
		//IList<T> ObtenerLista(int paginaNro, int tamPagina);
		T ObtenerPorId(int id);
	}
}
