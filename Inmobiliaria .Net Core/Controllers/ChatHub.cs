using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_.Net_Core.Controllers
{
	public class ChatHub : Hub
	{
		public override async Task OnConnectedAsync()
		{
			await base.OnConnectedAsync();
			await Clients.All.SendAsync("UsuarioConectado", Context.ConnectionId);
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			await Clients.All.SendAsync("UsuarioDesconectado", Context.ConnectionId);
		}

		public async Task SendMessage(string user, string message)
		{
			await Clients.All.SendAsync("ReceiveMessage", user, message);
		}

		public async Task SendMP(string user, string message, string connId)
		{
			await Clients.Client(connId).SendAsync("ReceiveMessage", user, message);
			await Clients.Caller.SendAsync("ReceiveMessage", user, message);
		}
	}
}
