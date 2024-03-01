namespace Backend.Entity.DTO.WebSocket;
using Backend.Entity.Concrete;

public class WebSocketRequest
{
    public string receiverUsername { get; set; }
    public string token { get; set; }
    public string message { get; set; }
}