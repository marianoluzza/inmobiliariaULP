using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public class RepositorioPersona
	{
		protected readonly string connectionString;
		public RepositorioPersona()
		{
			connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=BDInmobiliaria;Trusted_Connection=True;MultipleActiveResultSets=true";
		}

		public List<Persona> ObtenerTodas()
		{
			List<Persona> res = new List<Persona>();
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				string sql = @$"
					SELECT 
						{nameof(Persona.Id)}, 
						{nameof(Persona.Nombre)} 
					FROM Personas";
				using (SqlCommand comm = new SqlCommand(sql, conn))
				{
					conn.Open();
					var reader = comm.ExecuteReader();
					while (reader.Read())
					{
						res.Add(new Persona
						{
							Id = reader.GetInt32(nameof(Persona.Id)),
							Nombre = reader.GetString(nameof(Persona.Nombre)),
						});
					}
					conn.Close();
				}
			}
			return res;
		}
	}
}
