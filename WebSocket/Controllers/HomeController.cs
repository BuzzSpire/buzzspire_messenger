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
        private static List<WS.WebSocket> _connections = new List<WS.WebSocket>();
        
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
                _connections.Add(ws);

                var buffer = new byte[1024 * 4];
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                while (!result.CloseStatus.HasValue)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var brodcastMessage = new { message = message, date = DateTime.Now };
                    await Broadcast(JsonSerializer.Serialize(brodcastMessage));
                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.CloseStatus.HasValue)
                    {
                        _connections.Remove(ws);
                    }
                }
            }
        }
        
        private async Task Broadcast(string message)
        {
            Console.WriteLine(_connections.Count); 
            var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            foreach (var ws in _connections)
            {
                if (ws.State == WS.WebSocketState.Open)
                {
                    await ws.SendAsync(buffer, WS.WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
