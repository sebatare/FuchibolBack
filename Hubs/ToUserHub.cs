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
            _connectionManager.AddConnection(Context.UserIdentifier, Context.ConnectionId);

            // Notifica a todos los clientes sobre la actualización de usuarios conectados
            var users = _connectionManager.GetAllConnections();
            await Clients.All.SendAsync("UpdateUsers", users);

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
