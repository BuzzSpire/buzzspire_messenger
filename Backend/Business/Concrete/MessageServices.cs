using System.Net.WebSockets;
using System.Text;
using Backend.Business.Abstract;

namespace Backend.Business.Concrete;

public class MessageServices : IMessageServices
{

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

    public void SaveConnection(long id, WebSocket ws)
    {
    }

    public void RemoveConnection(long id)
    {
    }

    public Dictionary<long, WebSocket> GetConnections()
    {
        return new Dictionary<long, WebSocket>();
    }

    public long[] GetOnlineUsers()
    {
        return new long[0];
    }

    public long GetIdByConnection(WebSocket ws)
    {   
        return 0;
    }

    public async Task SendToUser(long senderId, string message, long receiverId)
    {
        // var ws = _connectionsDbContext.Get(receiverId);
        // if (ws.State == WebSocketState.Open)
        // {
        //     var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        //     await ws.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        // }
    }
}