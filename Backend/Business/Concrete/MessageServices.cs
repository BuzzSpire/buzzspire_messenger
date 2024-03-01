using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Backend.Business.Abstract;
using Backend.Data.Abstract;
using Backend.Data.Concrete.EF;
using Backend.Entity.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Business.Concrete;

public class MessageServices : IMessageServices
{
    private readonly IJwtServices _jwtServices;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IConnectionDb _connectionDb;
    private readonly IEncryptServices _encryptServices;

    public MessageServices(IJwtServices jwtServices, ApplicationDbContext applicationDbContext,
        IConnectionDb connectionDb, IEncryptServices encryptServices)
    {
        _jwtServices = jwtServices;
        _applicationDbContext = applicationDbContext;
        _connectionDb = connectionDb;
        _encryptServices = encryptServices;
    }


    public Task Broadcast(string message)
    {
        // var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        // foreach (var ws in GetConnections().ToList())
        // {
        //     if (ws.Value.State == WebSocketState.Open)
        //     {
        //         await ws.Value.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        //     }
        // }
        return Task.CompletedTask;
    }

    public Boolean SaveConnection(long id, WebSocket ws)
    {
        return _connectionDb.SaveConnection(id, ws);
    }

    public Boolean RemoveConnection(long id)
    {
        return _connectionDb.RemoveConnection(id);
    }

    public WebSocket GetConnections()
    {
        // TODO: Implement this method
        return null;
    }

    public long[] GetOnlineUsers()
    {
        return new long[0];
    }

    public long GetIdByConnection(WebSocket ws)
    {
        return _connectionDb.GetConnectionId(ws);
    }

    public async Task SendToUser(long senderId, string receiveUserName)
    {
        var receiverId = _applicationDbContext.Users
            .Where(u => u.UserName == receiveUserName)
            .Select(u => u.Id)
            .FirstOrDefault();

        var ws = _connectionDb.GetConnection(receiverId);

        var res = JsonSerializer.Serialize(new
        {
            receiverId,
            senderId,
            senderUserName = _applicationDbContext.Users
                .Where(u => u.Id == senderId)
                .Select(u => u.UserName)
                .FirstOrDefault(),
            message = true
        });

        if (ws.State == WebSocketState.Open)
        {
            await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(res)), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }
    }

    public async Task<IActionResult> SendMessage(string receiverUserName, string message, string token)
    {
        if (!_jwtServices.IsTokenValid(token))
        {
            return new UnauthorizedObjectResult(new { error = "Invalid token." });
        }

        var receiverUser = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserName == receiverUserName);
        if (receiverUser == null)
        {
            return new NotFoundObjectResult(new { error = "User not found." });
        }

        var senderId = _jwtServices.GetUserIdFromToken(token);
        

        _applicationDbContext.Messages.Add(new Message
        {
            SenderId = senderId,
            ReceiverId = receiverUser.Id,
            Content = _encryptServices.EncryptMessage(message),
            Date = DateTime.UtcNow,
        });

        try
        {
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(new { error = e.Message });
        }

        return new OkResult();
    }

    public async Task<IActionResult> GetMessages(string receiverUserName, string token, long page)
    {
        if (!_jwtServices.IsTokenValid(token))
        {
            return new UnauthorizedObjectResult(new { error = "Invalid token." });
        }

        var receiverUser = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserName == receiverUserName);
        if (receiverUser == null)
        {
            return new NotFoundObjectResult(new { error = "User not found." });
        }

        var senderId = _jwtServices.GetUserIdFromToken(token);
        
        var messages = await _applicationDbContext.Messages
            .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverUser.Id) ||
                        (m.SenderId == receiverUser.Id && m.ReceiverId == senderId))
            .OrderByDescending(m => m.Date)
            .Skip((int) (page - 1) * 10)
            .Take(10)
            .ToListAsync();
        
        messages.ForEach(m => m.Content = _encryptServices.DecryptMessage(m.Content));
        
        

        return new OkObjectResult(messages);
    }

    public async Task<IActionResult> GetAllLastMessagesGroupByUsers(string token)
    {
        if (!_jwtServices.IsTokenValid(token))
        {
            return new UnauthorizedObjectResult(new { error = "Invalid token." });
        }

        var userId = _jwtServices.GetUserIdFromToken(token);

        var messages = await _applicationDbContext.Messages
            .Where(m => m.SenderId == userId || m.ReceiverId == userId)
            .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
            .Select(g => new
            {
                ProfilePicture = _applicationDbContext.Users
                    .Where(u => u.Id == g.Key)
                    .Select(u => u.ProfilePicture)
                    .FirstOrDefault(),
                UserId = g.Key,
                UserName = _applicationDbContext.Users
                    .Where(u => u.Id == g.Key)
                    .Select(u => u.UserName)
                    .FirstOrDefault(),
                LastMessage = g.OrderByDescending(m => m.Date).FirstOrDefault()
            })
            .ToListAsync();
        
        messages.ForEach(m => m.LastMessage.Content = _encryptServices.DecryptMessage(m.LastMessage.Content));

        return new OkObjectResult(messages);
    }
}