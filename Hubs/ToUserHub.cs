using Microsoft.AspNetCore.SignalR;

namespace Fuchibol.ChatService.Hubs;

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

    //ESTA FUNCION SE EJECUTA CUANDO UN CLIENTE SE CONECTA AL HUB
	public override Task OnConnectedAsync()
		{
			// You can pass a user identifier if needed.
			_connectionManager.AddConnection(Context.UserIdentifier, Context.ConnectionId);
			return base.OnConnectedAsync();
		}

    public override Task OnDisconnectedAsync(Exception exception)
    {
        _connectionManager.RemoveConnection(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public IReadOnlyDictionary<string, string> GetConnectedClients()
    {
        return _connectionManager.GetAllConnections();
    }
}

