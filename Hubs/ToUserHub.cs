using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Fuchibol.ChatService.Hubs
{
	[Authorize]
	public class ToUserHub : Hub
	{
		private readonly ConnectionManager _connectionManager;

		public ToUserHub(ConnectionManager connectionManager)
		{
			_connectionManager = connectionManager;
;
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
        var email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        var connectionId = Context.ConnectionId;
		Console.WriteLine("CLAIMS");
        foreach (var claim in Context.User.Claims)
        {
            Console.WriteLine($"{claim.Type}: {claim.Value}");
        }

        if (email != null)
        {
            _connectionManager.AddConnection(email, connectionId);
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
