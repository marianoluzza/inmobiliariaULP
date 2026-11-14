namespace Inmobiliaria_.Net_Core.Services
{
	/// <summary>
	/// Resultado de comparar una clave ingresada contra el hash guardado.
	/// </summary>
	public enum ResultadoVerificacion
	{
		/// <summary>La clave no coincide (o no hay hash guardado).</summary>
		Incorrecta,
		/// <summary>La clave coincide y el hash está al día.</summary>
		Correcta,
		/// <summary>
		/// La clave coincide, pero el hash está en un formato viejo o con menos
		/// iteraciones de las actuales: conviene volver a hashearla y guardarla.
		/// </summary>
		CorrectaRequiereRehash,
	}

	/// <summary>
	/// Único lugar donde se hashean y verifican claves. Los controladores lo piden
	/// por inyección de dependencias en vez de llamar a KeyDerivation cada uno.
	/// </summary>
	public interface IServicioClaves
	{
		/// <summary>Devuelve el hash a guardar en la base para la clave plana.</summary>
		string Hashear(string clave);

		/// <summary>Compara la clave plana ingresada contra el hash guardado.</summary>
		ResultadoVerificacion Verificar(string? hashGuardado, string clave);
	}
}
