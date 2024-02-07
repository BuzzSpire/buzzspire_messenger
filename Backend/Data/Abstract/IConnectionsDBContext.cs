namespace Backend.Data.Abstract;
using System.Net.WebSockets;

public interface IConnectionsDbContext
{
    public void Add (string userName, WebSocket ws);
    public void Remove(string userName);
    public Dictionary<String, WebSocket> Get();
    public string[] GetKeys();
    public string Get(WebSocket ws);
}