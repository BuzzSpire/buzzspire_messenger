using System.Collections;

namespace WebSocket.Data.Abstract;
using WS = System.Net.WebSockets;

public interface IConnectionsDBContext
{
    public void Add (string userName, WS.WebSocket ws);
    public void Remove(string userName);
    public Dictionary<String, WS.WebSocket> Get();
}