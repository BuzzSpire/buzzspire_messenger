using System.Collections;

namespace WebSocket.Data.Abstract;
using WS = System.Net.WebSockets;

public interface IConnectionsDbContext
{
    public void Add (string userName, WS.WebSocket ws);
    public void Remove(string userName);
    public Dictionary<String, WS.WebSocket> Get();
    public string[] GetKeys();
    public string Get(WS.WebSocket ws);
}