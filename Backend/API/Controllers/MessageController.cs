using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Backend.Business.Abstract;
using Backend.Entity.DTO.Message;
using Backend.Entity.DTO.WebSocket;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IJwtServices _jwtServices;
    private readonly IMessageServices _messageServices;

    public MessageController(IMessageServices messageServices, IJwtServices jwtServices)
    {
        _messageServices = messageServices;
        _jwtServices = jwtServices;
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
                WebSocketRequest request =
                    JsonSerializer.Deserialize<WebSocketRequest>(Encoding.UTF8.GetString(buffer, 0, result.Count)) ??
                    throw new InvalidOperationException();


                if (!_jwtServices.IsTokenValid(request.token))
                {
                    return;
                }

                long userId = _jwtServices.GetUserIdFromToken(request.token);


                _messageServices.SaveConnection(userId, ws);


                if (request.receiverUsername != "")
                {
                    await _messageServices.SendToUser(userId, request.receiverUsername);
                }

                buffer = new byte[1024 * 4];
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
    public async Task<IActionResult> Send([FromHeader] string token, [FromBody] SendMessageRequest request)
    {
        return await _messageServices.SendMessage(request.ReceiverUsername, request.Content, token);
    }

    [HttpGet("{receiverusername}/{page}")]
    public async Task<IActionResult> GetMessages([FromHeader] string token, [FromRoute] string receiverusername,
        [FromRoute] long page)
    {
        return await _messageServices.GetMessages(receiverusername, token, page);
    }

    [HttpGet("getAllLastMessages")]
    public async Task<IActionResult> GetAllLastMessages([FromHeader] string token)
    {
        return await _messageServices.GetAllLastMessagesGroupByUsers(token);
    }
}