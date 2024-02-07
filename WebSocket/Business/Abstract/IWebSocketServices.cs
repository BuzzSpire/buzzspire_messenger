using WS = System.Net.WebSockets;

namespace WebSocket.Business.Abstract;

public interface IWebSocketServices
{
   public Task Broadcast(string message);
   public void SaveConnection(string userName, WS.WebSocket ws);
   public void RemoveConnection(string userName);
   public Dictionary<String, WS.WebSocket> GetConnections();
   public string[] GetOnlineUsers();
   public string GetUserNameByConnection(WS.WebSocket ws);
}