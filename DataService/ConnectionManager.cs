// ConnectionManager.cs
using System.Collections.Concurrent;

public class ConnectionManager
{
    private readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

    public void AddConnection(string userId, string connectionId)
    {
        Console.WriteLine("Funcion AddConection");
        Console.WriteLine(userId,connectionId);
        _connections[connectionId] = userId;
    }

    public void RemoveConnection(string connectionId)
    {
        _connections.TryRemove(connectionId, out _);
    }

    public IReadOnlyDictionary<string, string> GetAllConnections()
    {
        return _connections;
    }
}
