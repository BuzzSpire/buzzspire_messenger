using Backend.Data.Abstract;
using System.Net.WebSockets;

namespace Backend.Data.Concrete;

public class ConnectionsDbContext:IConnectionsDbContext
{
    private static Dictionary<String, WebSocket> _connections = new Dictionary<String, WebSocket>();
    
    public void Add(string userName, WebSocket ws)
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

    public Dictionary<string, WebSocket> Get()
    {
        return _connections;
    }

    public string[] GetKeys()
    {
        return _connections.Keys.ToArray();
    }

    public string Get(WebSocket ws)
    {
       return _connections.FirstOrDefault(x => x.Value == ws).Key;
    }
}