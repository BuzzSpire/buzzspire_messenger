using WS = System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace WebSocket.Controllers
{
    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {
        private static Dictionary<String, WS.WebSocket> _connections = new Dictionary<String, WS.WebSocket>();
       
        [HttpGet]
        public IActionResult Index()
        {
            return new JsonResult(new { message = "Welcome to WebSocket" });
        }

        [HttpGet("ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var ws = await HttpContext.WebSockets.AcceptWebSocketAsync();

                var buffer = new byte[1024 * 4];
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!result.CloseStatus.HasValue)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var messageJson = JsonSerializer.Deserialize<Dictionary<string, string>>(message);
                    
                    message = messageJson["message"];
                    var userName = messageJson["userName"];
                    
                    if (!_connections.ContainsKey(userName))
                    {
                        _connections.Add(userName, ws);
                    }
                    
                    var brodcastMessage = new {userName= userName ,message = message, date = DateTime.Now, onlineUsers = _connections.Keys.ToList()};
                    await Broadcast(JsonSerializer.Serialize(brodcastMessage));
                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.CloseStatus.HasValue)
                    {
                        _connections.Remove(userName);
                        await Broadcast(JsonSerializer.Serialize(new
                        {
                            userName = userName, message = "left the chat", date = DateTime.Now,
                            onlineUsers = _connections.Keys.ToList()
                        }));
                    }
                }
            }
        }
        
        private async Task Broadcast(string message)
        {
            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            foreach (var ws in _connections)
            {
                if (ws.Value.State == WS.WebSocketState.Open)
                {
                    await ws.Value.SendAsync(buffer, WS.WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
