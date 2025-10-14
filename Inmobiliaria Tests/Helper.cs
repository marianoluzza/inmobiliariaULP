using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.TestHost;
using System.Text;
using Inmobiliaria_.Net_Core.Api;

namespace Inmobiliaria_Tests
{
	class Helper
	{
		public Helper()
		{
			var valoresConfig = new Dictionary<string, string>
			{
				//{"ConnectionStrings:Testing", "Server=(localdb)\\MSSQLLocalDB;Database=BDInmoTest;Trusted_Connection=True;MultipleActiveResultSets=true"},
				{"ConnectionStrings:Testing", "Server=(localdb)\\MSSQLLocalDB;Integrated Security=true;AttachDbFileName=C:\\Git\\inmobiliariaULP\\Inmobiliaria .Net Core\\Data\\BDInmoTest.mdf"},
				{"ConnectionStrings:DefaultConnection", "Server=(localdb)\\MSSQLLocalDB;Database=BDInmobiliaria;Trusted_Connection=True;MultipleActiveResultSets=true"},
				{"Salt", "Salada"},
				{"TokenAuthentication:SecretKey", "Super_Secreta_es_la_clave_de_esta_APP_shhh"},
				{"TokenAuthentication:Issuer", "inmobiliariaULP"},
				{"TokenAuthentication:Audience", "mobileAPP"},
				{"TokenAuthentication:TokenPath", "/api/token"},
				{"TokenAuthentication:CookieName", "access_token"},
			};
			Config = new ConfigurationBuilder()
				.AddInMemoryCollection(valoresConfig)
				.Build();
			DataContext = new DataContext(new DbContextOptionsBuilder<DataContext>().UseSqlServer(Config["ConnectionStrings:Testing"]).Options);
		}

		internal IConfiguration Config { get; set; }
		internal DataContext DataContext { get; set; }

		internal ClaimsPrincipal MockLogin(string email, string rol)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, email),
				new Claim(ClaimTypes.Role, rol),
			};
			var identity = new ClaimsIdentity(claims, "TestAuthType");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			return claimsPrincipal;
		}

		// Construye y configura una WebApplication para tests (TestServer)
		public WebApplication BuildWebApplication()
		{
			var builder = WebApplication.CreateBuilder(System.Array.Empty<string>());
			builder.Configuration.AddConfiguration(Config);
			// Registrar controllers de la app principal
			builder.Services.AddControllers()
				.AddApplicationPart(typeof(PropietariosController).Assembly);
			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				var secreto = builder.Configuration["TokenAuthentication:SecretKey"];
				if (string.IsNullOrEmpty(secreto))
					throw new Exception("Falta configurar TokenAuthentication:Secret");
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = builder.Configuration["TokenAuthentication:Issuer"],
					ValidAudience = builder.Configuration["TokenAuthentication:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secreto)),
				};
			});
			builder.Services.AddAuthorization();
			builder.WebHost.UseTestServer();
			var appServer = builder.Build();
			appServer.UseRouting();
			appServer.UseAuthentication();
			appServer.UseAuthorization();
			appServer.MapControllers();
			return appServer;
		}

		// Crea una conexión SQLite in-memory y la deja abierta
		public static DbConnection CreateInMemoryDatabase()
		{
			var connection = new SqliteConnection("DataSource=:memory:;Cache=Shared");
			connection.Open();
			return connection;
		}

		public static DbContextOptions<DataContext> CreateOptions(DbConnection connection)
		{
			return new DbContextOptionsBuilder<DataContext>()
				.UseSqlite(connection)
				.Options;
		}
	}
}