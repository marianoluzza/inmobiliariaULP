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
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Inmuebles (Direccion, Ambientes, Superficie, Latitud, Longitud, PropietarioId) " +
					$"VALUES ('{entidad.Direccion}', '{entidad.Ambientes}','{entidad.Superficie}','{entidad.Latitud}','{entidad.Longitud}','{entidad.PropietarioId}')";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
                    command.CommandText = "SELECT SCOPE_IDENTITY()";
                    var id = command.ExecuteScalar();
                    entidad.Id = Convert.ToInt32(id);
                    connection.Close();
				}
			}
			return res;
		}
		public int Baja(int id)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"DELETE FROM Inmuebles WHERE Id = {id}";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificacion(Inmueble inmueble)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
                string sql = $"UPDATE Inmuebles SET Direccion=@direccion, Ambientes=@ambientes, Superficie=@superficie, Latitud=@latitud, Longitud=@longitud, PropietarioId=@propietarioId " +
					$"WHERE Id = {inmueble.Id}";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
                    command.Parameters.Add("@direccion", SqlDbType.VarChar).Value = inmueble.Direccion;
                    command.Parameters.Add("@ambientes", SqlDbType.Int).Value = inmueble.Ambientes;
                    command.Parameters.Add("@superficie", SqlDbType.Int).Value = inmueble.Superficie;
                    command.Parameters.Add("@latitud", SqlDbType.Decimal).Value = inmueble.Latitud;
                    command.Parameters.Add("@longitud", SqlDbType.Decimal).Value = inmueble.Longitud;
                    command.Parameters.Add("@propietarioId", SqlDbType.Int).Value = inmueble.PropietarioId;
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
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Direccion, Ambientes, Superficie, Latitud, Longitud, PropietarioId, p.Nombre, p.Apellido" +
                    $" FROM Inmuebles i INNER JOIN Propietarios p ON i.PropietarioId = p.IdPropietario";
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
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
                string sql = $"SELECT Id, Direccion, Ambientes, Superficie, Latitud, Longitud, PropietarioId, p.Nombre, p.Apellido" +
                    $" FROM Inmuebles i INNER JOIN Propietarios p ON i.PropietarioId = p.IdPropietario" +
                    $" WHERE Id=@id";
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
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Direccion, Ambientes, Superficie, Latitud, Longitud, PropietarioId, p.Nombre, p.Apellido" +
                    $" FROM Inmuebles i INNER JOIN Propietarios p ON i.PropietarioId = p.IdPropietario" +
                    $" WHERE PropietarioId=@idPropietario";
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
    }
}
