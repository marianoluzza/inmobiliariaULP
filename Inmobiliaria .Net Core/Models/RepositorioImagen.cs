using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public class RepositorioImagen : RepositorioBase, IRepositorioImagen
	{
		public RepositorioImagen(IConfiguration configuration) : base(configuration)
		{
		}

		public int Alta(Imagen p)
		{
			int res = -1;
			using (var connection = new SqlConnection(connectionString))
			{
				string sql = @"INSERT INTO Imagenes 
					(InmuebleId, Url) 
					VALUES (@inmuebleId, @url)";
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@inmuebleId", p.InmuebleId);
					command.Parameters.AddWithValue("@url", p.Url);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public int Baja(int id)
		{
			int res = -1;
			using (var connection = new SqlConnection(connectionString))
			{
				string sql = @$"DELETE FROM Imagenes WHERE Id = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public int Modificacion(Imagen p)
		{
			int res = -1;
			using (var connection = new SqlConnection(connectionString))
			{
				string sql = @"
				UPDATE Imagenes SET 
					Url=@url
				WHERE Id=@id";
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", p.Id);
					command.Parameters.AddWithValue("@url", p.Url);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public Imagen ObtenerPorId(int id)
		{
			Imagen res = null;
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				string sql = @$"
					SELECT 
						{nameof(Imagen.Id)}, 
						{nameof(Imagen.InmuebleId)}, 
						{nameof(Imagen.Url)} 
					FROM Imagenes
					WHERE {nameof(Imagen.Id)}=@id";
				using (SqlCommand comm = new SqlCommand(sql, conn))
				{
					comm.Parameters.AddWithValue("@id", id);
					conn.Open();
					var reader = comm.ExecuteReader();
					if (reader.Read())
					{
						res = new Imagen();
						res.Id = reader.GetInt32(nameof(Imagen.Id));
						res.InmuebleId = reader.GetInt32(nameof(Imagen.InmuebleId));
						res.Url = reader.GetString(nameof(Imagen.Url));
					}
					conn.Close();
				}
			}
			return res;
		}

		public IList<Imagen> ObtenerTodos()
		{
			List<Imagen> res = new List<Imagen>();
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				string sql = @$"
					SELECT 
						{nameof(Imagen.Id)}, 
						{nameof(Imagen.InmuebleId)}, 
						{nameof(Imagen.Url)} 
					FROM Imagenes";
				using (SqlCommand comm = new SqlCommand(sql, conn))
				{
					conn.Open();
					var reader = comm.ExecuteReader();
					while (reader.Read())
					{
						res.Add(new Imagen
						{
							Id = reader.GetInt32(nameof(Imagen.Id)),
							InmuebleId = reader.GetInt32(nameof(Imagen.InmuebleId)),
							Url = reader.GetString(nameof(Imagen.Url)),
						});
					}
					conn.Close();
				}
			}
			return res;
		}

		public IList<Imagen> BuscarPorInmueble(int inmuebleId)
		{
			List<Imagen> res = new List<Imagen>();
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				string sql = @$"
					SELECT 
						{nameof(Imagen.Id)}, 
						{nameof(Imagen.InmuebleId)}, 
						{nameof(Imagen.Url)} 
					FROM Imagenes
					WHERE {nameof(Imagen.InmuebleId)}=@inmuebleId";
				using (SqlCommand comm = new SqlCommand(sql, conn))
				{
					comm.Parameters.AddWithValue("@inmuebleId", inmuebleId);
					conn.Open();
					var reader = comm.ExecuteReader();
					while (reader.Read())
					{
						res.Add(new Imagen
						{
							Id = reader.GetInt32(nameof(Imagen.Id)),
							InmuebleId = reader.GetInt32(nameof(Imagen.InmuebleId)),
							Url = reader.GetString(nameof(Imagen.Url)),
						});
					}
					conn.Close();
				}
			}
			return res;
		}
	}
}
