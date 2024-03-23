using System.Net.WebSockets;
using Backend.Entity.Concrete;

namespace Backend.Data.Abstract;

public interface IConnectionDb
{
    public WebSocket  GetConnection(long userId);
    public void SaveConnection(long userId, WebSocket ws);
    public Boolean RemoveConnection(long userId);
    public long GetConnectionId(WebSocket ws);
    public Boolean IsUserOnline(long id);
}