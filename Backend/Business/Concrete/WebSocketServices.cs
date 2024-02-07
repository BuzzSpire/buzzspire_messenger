using System.Net.WebSockets;
using System.Text;
using Backend.Business.Abstract;
using Backend.Data.Abstract;

namespace Backend.Business.Concrete;

public class WebSocketServices : IWebSocketServices
{
    private IConnectionsDbContext _connectionsDbContext;

    public WebSocketServices(IConnectionsDbContext connectionsDbContext)
    {
        _connectionsDbContext = connectionsDbContext;
    }

    public async Task Broadcast(string message)
    {
        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        foreach (var ws in GetConnections().ToList())
        {
            if (ws.Value.State == WebSocketState.Open)
            {
                await ws.Value.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    public void SaveConnection(string userName, WebSocket ws)
    {
        _connectionsDbContext.Add(userName, ws);
    }

    public void RemoveConnection(string userName)
    {
        _connectionsDbContext.Remove(userName);
    }

    public Dictionary<string, WebSocket> GetConnections()
    {
        return _connectionsDbContext.Get();
    }

    public string[] GetOnlineUsers()
    {
        return _connectionsDbContext.GetKeys();
    }

    public string GetUserNameByConnection(WebSocket ws)
    {
        return _connectionsDbContext.Get(ws);
    }
}