using Inmobiliaria_.Net_Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Controllers
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	public class ChatSeguroHub : Hub<IChatClient>
	{
		private readonly DataContext context;

		//parámetros provistos por inyección de dependencias
		public ChatSeguroHub(DataContext context)
		{
			this.context = context;
		}
		public override async Task OnConnectedAsync()
		{
			try
			{
				await base.OnConnectedAsync();
				var p = await context.Propietarios.FirstAsync(x => x.Email == Context.UserIdentifier);
				var otros = await context.Conectados.Where(x => x.Usuario != Context.UserIdentifier).ToListAsync();
				var conectado = new Conectado(p);
				var yaConectado = await context.Conectados.FindAsync(Context.UserIdentifier);
				if (yaConectado == null)
				{
					await context.Conectados.AddAsync(conectado);
					await context.SaveChangesAsync();
				}
				await Clients.All.UsuarioConectado(conectado);
				foreach (var c in otros)
				{
					//notificar los demás conectados al reciente
					await Clients.Caller.UsuarioConectado(c);
				}
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			await base.OnDisconnectedAsync(exception);
			var c = await context.Conectados.FindAsync(Context.UserIdentifier);
			if (c != null)
			{
				context.Conectados.Remove(c);
				await context.SaveChangesAsync();
				await Clients.All.UsuarioDesconectado(c);
			}
		}

		public async Task SendMessage(Mensaje mje)
		{
			var c = await context.Conectados.FindAsync(Context.UserIdentifier);
			mje.Emisor = c.Nombre;
			if (String.IsNullOrEmpty(mje.Destinatario))
				await Clients.All.ReceiveMessage(mje);
			else
			{
				await Clients.User(mje.Destinatario).ReceiveMessage(mje);
				await Clients.Caller.ReceiveMessage(mje);
			}
		}
	}

	public interface IChatClient
	{
		Task ReceiveMessage(Mensaje mje);
		Task UsuarioConectado(Conectado usuario);
		Task UsuarioDesconectado(Conectado usuario);
	}

	public class UserIdProvider : IUserIdProvider
	{
		public string GetUserId(HubConnectionContext connection)
		{
			//por default es System.Security.Claims.ClaimTypes.NameIdentifier así que lo cambiamos a Name			
			return connection.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
			// if(connection.User != null) //equivalente a la línea anterior
			// {
			// 	var res = connection.User.FindFirst(System.Security.Claims.ClaimTypes.Name);
			// 	if(res != null)
			// 		return res.Value;
			// }
			// return null;
		}
	}
}
