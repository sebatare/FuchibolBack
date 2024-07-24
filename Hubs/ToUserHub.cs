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

        // Formatear los claims en un string legible
        string claimsString = FormatClaims(Context.User);

        // Imprimir los claims
        Console.WriteLine(claimsString);

        if (email != null)
        {
            _connectionManager.AddConnection(email, connectionId);
            await Clients.All.SendAsync("UpdateUsers", _connectionManager.GetAllConnections());
        }

        await base.OnConnectedAsync();
    }

    private string FormatClaims(ClaimsPrincipal user)
    {
        var claims = user.Claims.Select(c => new { c.Type, c.Value });
        return System.Text.Json.JsonSerializer.Serialize(claims);
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
