using WebSocket.Data.Abstract;
using WS = System.Net.WebSockets;

namespace WebSocket.Data.Concrete;

public class ConnectionsDbContext:IConnectionsDbContext
{
    private static Dictionary<String, WS.WebSocket> _connections = new Dictionary<String, WS.WebSocket>();
    
    public void Add(string userName, WS.WebSocket ws)
    {
        if (!_connections.ContainsKey(userName) )
        {
            _connections.Add(userName, ws);
        }
    }

    public void Remove(string userName)
    {
        if (_connections.ContainsKey(userName))
        {
            _connections.Remove(userName);
        }
    }

    public Dictionary<string, WS.WebSocket> Get()
    {
        return _connections;
    }

    public string[] GetKeys()
    {
        return _connections.Keys.ToArray();
    }

    public string Get(WS.WebSocket ws)
    {
       return _connections.FirstOrDefault(x => x.Value == ws).Key;
    }
}