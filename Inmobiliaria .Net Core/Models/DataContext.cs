﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{

		}
		public DbSet<Propietario> Propietarios { get; set; }
		public DbSet<Inquilino> Inquilinos { get; set; }
		public DbSet<Inmueble> Inmuebles { get; set; }

		public DbSet<Persona> Personas { get; set; }
		public DbSet<Pasatiempo> Pasatiempos { get; set; }
		public DbSet<PersonaPasatiempo> PersonaPasatiempos { get; set; }

		public DbSet<Conectado> Conectados { get; set; }
		public DbSet<Usuario> Usuarios { get; set; }
	}
}
