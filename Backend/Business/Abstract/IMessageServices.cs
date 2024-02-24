using System.Net.WebSockets;
using Backend.Entity.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Business.Abstract;

public interface IMessageServices
{
   public Task Broadcast(string message);
   public Boolean SaveConnection(long id, WebSocket ws);
   public Boolean RemoveConnection(long id);
   public WebSocket GetConnections();
   public long[] GetOnlineUsers();
   public long GetIdByConnection(WebSocket ws);
   public Task SendToUser(long senderId, string receiverUserName );
   public Task<IActionResult> SendMessage(string receiverUserName, string message, string token);
   public Task<IActionResult> GetMessages(string receiverUserName, string token, long page);
   public Task<IActionResult> GetAllLastMessagesGroupByUsers(string token);
}