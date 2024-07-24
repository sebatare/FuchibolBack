using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Fuchibol.ChatService.Hubs
{
	public class ToUserHub : Hub
	{
		private readonly ConnectionManager _connectionManager;

		public ToUserHub(ConnectionManager connectionManager)
		{
			_connectionManager = connectionManager;
		}

		public async Task SendMessage(string user, string message)
		{
			await Clients.All.SendAsync("F", user, message);
		}

		public async Task SendToUser(string user, string receiverConnectionId, string message)
		{
			await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", user, message);
		}

		public string GetConnectionId() => Context.ConnectionId;

		public override async Task OnConnectedAsync()
		{
			var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var connectionId = Context.ConnectionId;

            if (userId != null)
            {
                _connectionManager.AddConnection(userId, connectionId);
                await Clients.All.SendAsync("UpdateUsers", _connectionManager.GetAllConnections());
            }
			await base.OnConnectedAsync();
		}

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			_connectionManager.RemoveConnection(Context.ConnectionId);

			// Notifica a todos los clientes sobre la actualización de usuarios conectados
			var users = _connectionManager.GetAllConnections();
			await Clients.All.SendAsync("UpdateUsers", users);

			await base.OnDisconnectedAsync(exception);
		}

		public IReadOnlyDictionary<string, string> GetConnectedClients()
		{
			return _connectionManager.GetAllConnections();
		}
	}
}
