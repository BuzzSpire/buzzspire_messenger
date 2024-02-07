using System.Text;
using WebSocket.Business.Abstract;
using WebSocket.Data.Abstract;
using WS = System.Net.WebSockets;

namespace WebSocket.Business.Concrete;

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
            if (ws.Value.State == WS.WebSocketState.Open)
            {
                await ws.Value.SendAsync(buffer, WS.WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    public void SaveConnection(string userName, WS.WebSocket ws)
    {
        _connectionsDbContext.Add(userName, ws);
    }

    public void RemoveConnection(string userName)
    {
        _connectionsDbContext.Remove(userName);
    }

    public Dictionary<string, WS.WebSocket> GetConnections()
    {
        return _connectionsDbContext.Get();
    }

    public string[] GetOnlineUsers()
    {
        return _connectionsDbContext.GetKeys();
    }

    public string GetUserNameByConnection(WS.WebSocket ws)
    {
        return _connectionsDbContext.Get(ws);
    }
}