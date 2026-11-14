namespace Inmobiliaria_.Net_Core.Models
{
	/// <summary>
	/// Lo que se devuelve de un propietario hacia afuera (JSON). Es igual a Propietario pero
	/// sin la clave: aunque esté hasheada, no tiene que salir del servidor.
	/// Las propiedades se llaman igual que en Propietario, así el JSON no cambia para quien lo usa.
	/// </summary>
	public class PropietarioView
	{
		public int IdPropietario { get; set; }
		public string Nombre { get; set; } = "";
		public string Apellido { get; set; } = "";
		public string Dni { get; set; } = "";
		public string Telefono { get; set; } = "";
		public string Email { get; set; } = "";

		public PropietarioView() { }

		public PropietarioView(Propietario p)
		{
			IdPropietario = p.IdPropietario;
			Nombre = p.Nombre;
			Apellido = p.Apellido;
			Dni = p.Dni;
			Telefono = p.Telefono;
			Email = p.Email;
		}
	}
}
