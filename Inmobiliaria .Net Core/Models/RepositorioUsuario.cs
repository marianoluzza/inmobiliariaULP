using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Models
{
	public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
	{
		public RepositorioUsuario(IConfiguration configuration) : base(configuration)
		{

		}

		public int Alta(Usuario e)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Usuarios (Nombre, Apellido, Avatar, Email, Clave, Rol) " +
					$"VALUES (@nombre, @apellido, @avatar, @email, @clave, @rol);" +
					"SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", e.Nombre);
					command.Parameters.AddWithValue("@apellido", e.Apellido);
					command.Parameters.AddWithValue("@avatar", e.Avatar);
					command.Parameters.AddWithValue("@email", e.Email);
					command.Parameters.AddWithValue("@clave", e.Clave);
					command.Parameters.AddWithValue("@rol", e.Rol);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
                    e.Id = res;
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
				string sql = $"DELETE FROM Usuarios WHERE Id = @id";
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
		public int Modificacion(Usuario e)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"UPDATE Usuarios SET Nombre=@nombre, Apellido=@apellido, Avatar=@avatar, Email=@email, Clave=@clave, Rol=@rol " +
					$"WHERE Id = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", e.Nombre);
					command.Parameters.AddWithValue("@apellido", e.Apellido);
					command.Parameters.AddWithValue("@avatar", e.Avatar);
					command.Parameters.AddWithValue("@email", e.Email);
					command.Parameters.AddWithValue("@clave", e.Clave);
					command.Parameters.AddWithValue("@rol", e.Rol);
					command.Parameters.AddWithValue("@id", e.Id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Usuario> ObtenerTodos()
		{
			IList<Usuario> res = new List<Usuario>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Avatar, Email, Clave, Rol" +
                    $" FROM Usuarios";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Usuario e = new Usuario
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Avatar = reader["Avatar"].ToString(),
							Email = reader.GetString(4),
							Clave = reader.GetString(5),
							Rol = reader.GetInt32(6),
						};
						res.Add(e);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Usuario ObtenerPorId(int id)
		{
			Usuario e = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT Id, Nombre, Apellido, Avatar, Email, Clave, Rol FROM Usuarios" +
					$" WHERE Id=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						e = new Usuario
						{
							Id = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Avatar = reader["Avatar"].ToString(),
							Email = reader.GetString(4),
							Clave = reader.GetString(5),
							Rol = reader.GetInt32(6),
						};
					}
					connection.Close();
				}
			}
			return e;
        }

        public Usuario ObtenerPorEmail(string email)
        {
            Usuario e = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Apellido, Avatar, Email, Clave, Rol FROM Usuarios" +
                    $" WHERE Email=@email";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        e = new Usuario
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Avatar = reader["Avatar"].ToString(),
                            Email = reader.GetString(4),
                            Clave = reader.GetString(5),
							Rol = reader.GetInt32(6),
						};
                    }
                    connection.Close();
                }
            }
            return e;
        }
    }
}
