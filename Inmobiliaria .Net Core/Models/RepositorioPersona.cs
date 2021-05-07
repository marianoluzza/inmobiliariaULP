using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public class RepositorioPersona : RepositorioBase
	{
		public RepositorioPersona(IConfiguration configuration) : base(configuration)
		{
			
		}

		public List<Persona> ObtenerTodas()
		{
			List<Persona> res = new List<Persona>();
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				string sql = "SELECT Id, Nombre FROM Personas";
				using (SqlCommand comm = new SqlCommand(sql, conn))
				{
					conn.Open();
					var reader = comm.ExecuteReader();
					while (reader.Read())
					{
						res.Add(new Persona
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
						});
					}
					conn.Close();
				}
			}
			return res;
		}
	}
}
