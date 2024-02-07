using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Backend.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("/message/")]
public class MessageController: ControllerBase
{
    private readonly IWebSocketServices _webSocketServices;
    
    public MessageController(IWebSocketServices webSocketServices)
    {
        _webSocketServices = webSocketServices;
    }
    
    [HttpGet("ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            WebSocket ws = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result =
                await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Dictionary<string, string>? messageJson =
                    JsonSerializer.Deserialize<Dictionary<string, string>>(message);

                if (messageJson != null)
                {
                    string userName = messageJson["userName"];

                    _webSocketServices.SaveConnection(userName, ws);

                    string broadcastMessage = JsonSerializer.Serialize(new
                    {
                        userName,
                        message = messageJson["message"],
                        date = DateTime.Now.ToString("HH:mm"),
                        onlineUsers = _webSocketServices.GetOnlineUsers()
                    });
                    await _webSocketServices.Broadcast(broadcastMessage);
                }

                result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            if (result.CloseStatus.HasValue)
            {
                string closedUserName = _webSocketServices.GetUserNameByConnection(ws);
                _webSocketServices.RemoveConnection(closedUserName);
                await _webSocketServices.Broadcast(JsonSerializer.Serialize(new
                {
                    userName = closedUserName,
                    message = "left the chat",
                    date = DateTime.Now,
                    onlineUsers = _webSocketServices.GetOnlineUsers()
                }));
            }

            await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}