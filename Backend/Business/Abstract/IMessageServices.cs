using System.Net.WebSockets;

namespace Backend.Business.Abstract;

public interface IMessageServices
{
   public Task Broadcast(string message);
   public void SaveConnection(long id, WebSocket ws);
   public void RemoveConnection(long id);
   public Dictionary<long, WebSocket> GetConnections();
   public long[] GetOnlineUsers();
   public long GetIdByConnection(WebSocket ws);
   public Task SendToUser(long senderId, string message,long receiverId );
}