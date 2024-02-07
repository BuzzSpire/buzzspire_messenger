using System.Net.WebSockets;

namespace Backend.Business.Abstract;

public interface IWebSocketServices
{
   public Task Broadcast(string message);
   public void SaveConnection(string userName, WebSocket ws);
   public void RemoveConnection(string userName);
   public Dictionary<String, WebSocket> GetConnections();
   public string[] GetOnlineUsers();
   public string GetUserNameByConnection(WebSocket ws);
}