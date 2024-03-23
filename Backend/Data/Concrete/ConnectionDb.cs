using System.Net.WebSockets;
using Backend.Data.Abstract;

namespace Backend.Data.Concrete;

public class ConnectionDb : IConnectionDb
{
    private static Dictionary<long, WebSocket> _connections = new Dictionary<long, WebSocket>();


    public WebSocket GetConnection(long userId)
    {
        var connection = _connections.FirstOrDefault(x => x.Key == userId);
        return connection.Value;
    }

    public void SaveConnection(long userId, WebSocket ws)
    {
        if (_connections.ContainsKey(userId))
        {
            _connections[userId] = ws;
        }
        else
        {
            _connections.Add(userId, ws);
        }
    }

    public Boolean RemoveConnection(long userId)
    {
        if (_connections.ContainsKey(userId))
        {
            _connections.Remove(userId);
            return true;
        }

        return false;
    }

    public long GetConnectionId(WebSocket ws)
    {
        return _connections.FirstOrDefault(x => x.Value == ws).Key;
    }

    public bool IsUserOnline(long id)
    {
        return _connections.ContainsKey(id);
    }
}