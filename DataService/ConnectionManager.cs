using System.Collections.Concurrent;

public class ConnectionManager
{
    private readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

    public void AddConnection(string email, string connectionId)
    {
        Console.WriteLine("Funcion AddConnection");
        Console.WriteLine($"Email: {email}, ConnectionId: {connectionId}");
        _connections[connectionId] = email;
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
