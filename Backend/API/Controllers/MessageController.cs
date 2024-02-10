using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Backend.Business.Abstract;
using Backend.Entity.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageServices _messageServices;

    public MessageController(IMessageServices messageServices)
    {
        _messageServices = messageServices;
    }

    [HttpGet("ws")]
    public async Task Get([FromHeader] string token)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            WebSocket ws = await HttpContext.WebSockets.AcceptWebSocketAsync();

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result =
                await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                Message message =
                    JsonSerializer.Deserialize<Message>(Encoding.UTF8.GetString(buffer, 0, result.Count)) ??
                    throw new InvalidOperationException();

                _messageServices.SaveConnection(message.SenderId, ws);

                string broadcastMessage = JsonSerializer.Serialize(new Message
                {
                    SenderId = message.SenderId,
                    Content = message.Content,
                    Date = DateTime.Now.ToString("HH:mm"),
                    ReceiverId = message.ReceiverId
                });
                await _messageServices.SendToUser(message.SenderId, broadcastMessage, message.ReceiverId);

                result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            if (result.CloseStatus.HasValue)
            {
                long closedUser = _messageServices.GetIdByConnection(ws);
                _messageServices.RemoveConnection(closedUser);
            }

            await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
    
    [HttpPost("send")]
    public async Task<ActionResult> Send()
    {
        return Ok();
    }
}