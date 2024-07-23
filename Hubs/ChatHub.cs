using Fuchibol.ChatService.DataService;
using Fuchibol.ChatService.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Fuchibol.ChatService.Hubs;

public class ChatHub : Hub
{

	private readonly SharedDb _shared;
	private static ConcurrentDictionary<string, string> _userConnections = new();

	private static ConcurrentDictionary<string, List<string>> _offlineMessages = new ConcurrentDictionary<string, List<string>>();

	public ChatHub(SharedDb shared) => _shared = shared;
	public async Task JoinChat(UserConnection conn)
	{
		//Notifico a todos los clientes que se ha unido un usuario
		//El RecieveMessage es el metodo que sera llamado a los clientes
		await Clients.All.SendAsync("ReceiveMessage", "admin", $"{conn.Username} ha entrado");
	}

	public async Task JoinSpecificChatRoom(UserConnection conn)
	{
		//Añado al usuario (Context.ConnectinId) al grupo de chat especificado (conn.ChatRoom)
		await Groups.AddToGroupAsync(Context.ConnectionId, conn.ChatRoom);

		_shared.connections[Context.ConnectionId] = conn;
		Console.WriteLine($"Usuario añadido: {conn.Username} en la sala: {conn.ChatRoom}");
		// Imprimir todas las conexiones
		PrintConnections();

		//Notifico al grupo (conn.ChatRoom) que se agrego un usuario.
		//await Clients.Group(conn.ChatRoom).SendAsync("JoinSpecificChatRoom", "admin", $"{conn.Username} ha entrado a {conn.ChatRoom}");
		// Notificar a todos los usuarios
		await Clients.All.SendAsync("JoinSpecificChatRoom", "admin", $"{conn.Username} ha entrado a {conn.ChatRoom}");
	}

	public async Task SendMessage(string msg)
	{
		if (_shared.connections.TryGetValue(Context.ConnectionId, out UserConnection conn)){
		await Clients.Group(conn.ChatRoom)
		.SendAsync("ReceiveSpecificMessage",conn.Username, msg);
		}
			}

	public void PrintConnections()
	{
	foreach (var connection in _shared.connections)
		{
			Console.WriteLine($"Connection ID: {connection.Key}, Username: {connection.Value.Username}, ChatRoom: {connection.Value.ChatRoom}");
		}
	}
	public Task SendMessageToUser(string userId, string message)
		{
			if (_userConnections.TryGetValue(userId, out var connectionId))
			{
				return Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
			}
			else
			{
				if (!_offlineMessages.ContainsKey(userId))
				{
					_offlineMessages[userId] = new List<string>();
				}
				_offlineMessages[userId].Add(message);
			}

			return Task.CompletedTask;
		}

}
