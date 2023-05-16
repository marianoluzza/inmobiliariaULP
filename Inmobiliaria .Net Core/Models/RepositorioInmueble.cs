using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
	{
		public RepositorioInmueble(IConfiguration configuration) : base(configuration)
		{

		}

		public int Alta(Inmueble entidad)
		{
			int res = -1;
			using (var connection = new SqlConnection(connectionString))
			{
				string sql = @"INSERT INTO Inmuebles 
					(Direccion, Ambientes, Superficie, Latitud, Longitud, PropietarioId)
					VALUES (@direccion, @ambientes, @superficie, @latitud, @longitud, @propietarioId);
					SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@direccion", entidad.Direccion);
					command.Parameters.AddWithValue("@ambientes", entidad.Ambientes);
					command.Parameters.AddWithValue("@superficie", entidad.Superficie);
					command.Parameters.AddWithValue("@latitud", entidad.Latitud);
					command.Parameters.AddWithValue("@longitud", entidad.Longitud);
					command.Parameters.AddWithValue("@propietarioId", entidad.PropietarioId);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					entidad.Id = res;
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
				string sql = @$"DELETE FROM Inmuebles WHERE Id = @id";
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
		public int Modificacion(Inmueble entidad)
		{
			int res = -1;
			using (var connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Inmuebles SET " +
	"Direccion=@direccion, Ambientes=@ambientes, Superficie=@superficie, Latitud=@latitud, Longitud=@longitud, PropietarioId=@propietarioId " +
	"WHERE Id = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@direccion", entidad.Direccion);
					command.Parameters.AddWithValue("@ambientes", entidad.Ambientes);
					command.Parameters.AddWithValue("@superficie", entidad.Superficie);
					command.Parameters.AddWithValue("@latitud", entidad.Latitud);
					command.Parameters.AddWithValue("@longitud", entidad.Longitud);
					command.Parameters.AddWithValue("@propietarioId", entidad.PropietarioId);
					command.Parameters.AddWithValue("@id", entidad.Id);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inmueble> ObtenerTodos()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (var connection = new SqlConnection(connectionString))
			{
				string sql = @"SELECT Id, Direccion, Ambientes, Superficie, Latitud, Longitud, PropietarioId,
					p.Nombre, p.Apellido
					FROM Inmuebles i INNER JOIN Propietarios p ON i.PropietarioId = p.IdPropietario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble entidad = new Inmueble
						{
							Id = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Ambientes = reader.GetInt32(2),
							Superficie = reader.GetInt32(3),
							Latitud = reader.GetDecimal(4),
							Longitud = reader.GetDecimal(5),
							PropietarioId = reader.GetInt32(6),
							Duenio = new Propietario
							{
								IdPropietario = reader.GetInt32(6),
								Nombre = reader.GetString(7),
								Apellido = reader.GetString(8),
							}
						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Inmueble ObtenerPorId(int id)
		{
			Inmueble entidad = null;
			using (var connection = new SqlConnection(connectionString))
			{
				string sql = @$"
					SELECT {nameof(Inmueble.Id)}, Direccion, Ambientes, Superficie, Latitud, Longitud, PropietarioId, p.Nombre, p.Apellido
					FROM Inmuebles i JOIN Propietarios p ON i.PropietarioId = p.IdPropietario
					WHERE {nameof(Inmueble.Id)}=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						entidad = new Inmueble
						{
							Id = reader.GetInt32(nameof(Inmueble.Id)),
							Direccion = reader.GetString("Direccion"),
							Ambientes = reader.GetInt32("Ambientes"),
							Superficie = reader.GetInt32("Superficie"),
							Latitud = reader.GetDecimal("Latitud"),
							Longitud = reader.GetDecimal("Longitud"),
							PropietarioId = reader.GetInt32("PropietarioId"),
							Duenio = new Propietario
							{
								IdPropietario = reader.GetInt32("PropietarioId"),
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido"),
							}
						};
					}
					connection.Close();
				}
			}
			return entidad;
		}

		public IList<Inmueble> BuscarPorPropietario(int idPropietario)
		{
			List<Inmueble> res = new List<Inmueble>();
			Inmueble entidad = null;
			using (var connection = new SqlConnection(connectionString))
			{
				string sql = @$"
					SELECT {nameof(Inmueble.Id)}, Direccion, Ambientes, Superficie, Latitud, Longitud, PropietarioId, p.Nombre, p.Apellido
					FROM Inmuebles i JOIN Propietarios p ON i.PropietarioId = p.IdPropietario
					WHERE PropietarioId=@idPropietario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idPropietario", SqlDbType.Int).Value = idPropietario;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						entidad = new Inmueble
						{
							Id = reader.GetInt32(nameof(Inmueble.Id)),
							Direccion = reader.GetString("Direccion"),
							Ambientes = reader.GetInt32("Ambientes"),
							Superficie = reader.GetInt32("Superficie"),
							Latitud = reader.GetDecimal("Latitud"),
							Longitud = reader.GetDecimal("Longitud"),
							PropietarioId = reader.GetInt32("PropietarioId"),
							Duenio = new Propietario
							{
								IdPropietario = reader.GetInt32("PropietarioId"),
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido"),
							}
						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}
	}
}
