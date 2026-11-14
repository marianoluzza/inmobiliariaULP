using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Inmobiliaria_.Net_Core.Services
{
	/// <summary>
	/// Hashea con el PasswordHasher de ASP.NET Core Identity (viene con el framework, no
	/// hace falta instalar nada): PBKDF2-HMACSHA512 con una sal aleatoria por clave. El
	/// texto guardado incluye versión, algoritmo, iteraciones y sal, así que se puede
	/// subir el costo más adelante sin romper las claves existentes.
	///
	/// También reconoce el formato anterior (PBKDF2-HMACSHA1, 1000 iteraciones, misma sal
	/// para todos, tomada de "Salt" en appsettings) para que nadie pierda el acceso: esas
	/// claves se verifican y se informa que hay que rehashearlas, cosa que hacen los
	/// logins. Cuando no quede ninguna clave de 44 caracteres en la base, se puede borrar
	/// VerificarLegado y la entrada "Salt" de la configuración.
	/// </summary>
	public class ServicioClaves : IServicioClaves
	{
		/// <summary>
		/// Valor recomendado por OWASP para PBKDF2-HMACSHA512
		/// (https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html).
		/// Si se sube, las claves guardadas con menos iteraciones devuelven
		/// CorrectaRequiereRehash y se actualizan en el próximo login.
		/// </summary>
		public const int Iteraciones = 210_000;

		// El formato anterior era un PBKDF2 de 256 bits sin nada más: 32 bytes (44 en base64).
		// Los del PasswordHasher miden 61 bytes o más, así que el largo alcanza para distinguirlos.
		private const int BytesHashLegado = 256 / 8;

		// PasswordHasher<TUser> recibe el usuario pero no lo usa; se pasa siempre el mismo.
		private static readonly object usuario = new();

		private readonly PasswordHasher<object> hasher = new(Options.Create(new PasswordHasherOptions
		{
			CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3,
			IterationCount = Iteraciones,
		}));

		private readonly byte[] saltLegado;

		public ServicioClaves(IConfiguration configuration)
		{
			// Igual que el código anterior: si no había "Salt" configurado, se usaba "".
			saltLegado = System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? "");
			hashRelleno = hasher.HashPassword(usuario, Guid.NewGuid().ToString());
		}

		// Hash descartable: cuando no hay hash guardado (p. ej. el email no existe) se verifica
		// igual contra este, para que la respuesta tarde lo mismo y no delate qué emails existen.
		private readonly string hashRelleno;

		public string Hashear(string clave)
		{
			ArgumentNullException.ThrowIfNull(clave);
			return hasher.HashPassword(usuario, clave);
		}

		public ResultadoVerificacion Verificar(string? hashGuardado, string clave)
		{
			var bytes = new byte[hashGuardado?.Length ?? 0];
			if (string.IsNullOrEmpty(hashGuardado) || clave is null
				|| !Convert.TryFromBase64String(hashGuardado, bytes, out int largo))
			{
				hasher.VerifyHashedPassword(usuario, hashRelleno, clave ?? "");
				return ResultadoVerificacion.Incorrecta;
			}

			if (largo == BytesHashLegado)
				return VerificarLegado(bytes.AsSpan(0, largo), clave);

			return hasher.VerifyHashedPassword(usuario, hashGuardado, clave) switch
			{
				PasswordVerificationResult.Success => ResultadoVerificacion.Correcta,
				PasswordVerificationResult.SuccessRehashNeeded => ResultadoVerificacion.CorrectaRequiereRehash,
				_ => ResultadoVerificacion.Incorrecta,
			};
		}

		private ResultadoVerificacion VerificarLegado(ReadOnlySpan<byte> guardado, string clave)
		{
			var calculado = KeyDerivation.Pbkdf2(
				password: clave,
				salt: saltLegado,
				prf: KeyDerivationPrf.HMACSHA1,
				iterationCount: 1000,
				numBytesRequested: BytesHashLegado);
			// El formato viejo es mucho más barato que el actual: sin esto, un email con clave
			// vieja respondería más rápido que uno inexistente y delataría que existe.
			hasher.VerifyHashedPassword(usuario, hashRelleno, clave);
			// Comparación en tiempo constante: no revela cuántos bytes coincidieron.
			return CryptographicOperations.FixedTimeEquals(calculado, guardado)
				? ResultadoVerificacion.CorrectaRequiereRehash
				: ResultadoVerificacion.Incorrecta;
		}
	}
}
