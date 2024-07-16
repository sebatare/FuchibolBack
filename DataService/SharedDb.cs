using System.Collections.Concurrent;
using Fuchibol.ChatService.Models;

namespace Fuchibol.ChatService.DataService;

public class SharedDb{
	private readonly ConcurrentDictionary<string, UserConnection> _connections = new();

	public ConcurrentDictionary<string, UserConnection> connections =>_connections;
}
