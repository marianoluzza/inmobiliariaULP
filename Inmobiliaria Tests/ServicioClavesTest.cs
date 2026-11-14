using Inmobiliaria_.Net_Core.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Xunit;

namespace Inmobiliaria_Tests
{
	public class ServicioClavesTest
	{
		/// <summary>
		/// Hash con el algoritmo anterior (PBKDF2-HMACSHA1, 1000 iteraciones, sal fija de
		/// configuración). Es el formato de las claves que ya están en la base.
		/// </summary>
		internal static string HashLegado(string clave) =>
			Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: clave,
				salt: System.Text.Encoding.ASCII.GetBytes("Salada"),
				prf: KeyDerivationPrf.HMACSHA1,
				iterationCount: 1000,
				numBytesRequested: 256 / 8));

		internal static ServicioClaves Crear()
		{
			var config = new ConfigurationBuilder()
				.AddInMemoryCollection(new Dictionary<string, string?> { ["Salt"] = "Salada" })
				.Build();
			return new ServicioClaves(config);
		}

		[Fact]
		public void Hashear_LuegoVerificar_ClaveCorrecta()
		{
			var servicio = Crear();
			var hash = servicio.Hashear("secreta");
			Assert.NotEqual("secreta", hash);
			Assert.Equal(ResultadoVerificacion.Correcta, servicio.Verificar(hash, "secreta"));
		}

		[Fact]
		public void Verificar_ClaveDistinta_Incorrecta()
		{
			var servicio = Crear();
			var hash = servicio.Hashear("secreta");
			Assert.Equal(ResultadoVerificacion.Incorrecta, servicio.Verificar(hash, "Secreta"));
		}

		[Fact]
		public void Verificar_HashLegado_SigueAndandoYPideRehash()
		{
			var servicio = Crear();
			Assert.Equal(ResultadoVerificacion.CorrectaRequiereRehash, servicio.Verificar(HashLegado("vieja"), "vieja"));
			Assert.Equal(ResultadoVerificacion.Incorrecta, servicio.Verificar(HashLegado("vieja"), "otra"));
		}
	}
}
