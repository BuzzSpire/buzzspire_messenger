using WS = System.Net.WebSockets;

namespace WebSocket.Data.Abstract;

public class ConnectionsDbContext:IConnectionsDBContext
{
    private static Dictionary<String, WS.WebSocket> _connections = new Dictionary<String, WS.WebSocket>();
    
    public void Add(string userName, WS.WebSocket ws)
    {
        if (_connections.ContainsKey(userName) )
        {
            _connections.Add(userName, ws);
        }
    }

    public void Remove(string userName)
    {
        _connections.Remove(userName);
    }

    public Dictionary<string, WS.WebSocket> Get()
    {
        return _connections;
    }
}