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

    public void SaveConnection(long id, WebSocket ws)
    {
        _connectionDb.SaveConnection(id, ws);
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

        var receiverWs = _connectionDb.GetConnection(receiverId);
        var senderWs = _connectionDb.GetConnection(senderId);

        if (receiverWs == null || senderWs == null)
        {
            return;
        }

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

        if (receiverWs.State == WebSocketState.Open && senderWs.State == WebSocketState.Open)
        {
            await receiverWs.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(res)), WebSocketMessageType.Text,
                true,
                CancellationToken.None);
            await senderWs.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(res)), WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }

    public async Task<IActionResult> SendMessage(string receiverUserName, string message, string token)
    {
        _jwtServices.ValidateToken(token);

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
        _jwtServices.ValidateToken(token);

        var senderUser = await _applicationDbContext.Users
            .Include(user => user.ReceivedMessages)
            .FirstOrDefaultAsync(u => u.UserName == receiverUserName);

        var receiverUser = await _applicationDbContext.Users
            .Include(u => u.ReceivedMessages)
            .FirstOrDefaultAsync(u => u.Id == _jwtServices.GetUserIdFromToken(token));

        if (receiverUser == null || senderUser == null)
        {
            return new NotFoundObjectResult(new { error = "User not found." });
        }

        var senderMessages = senderUser.ReceivedMessages
            .Where(m => m.SenderId == receiverUser.Id)
            .Select(m => new
            {
                m.SenderId,
                m.ReceiverId,
                m.Content,
                m.Date,
                UserName = receiverUser.UserName,
                FullName = receiverUser.FullName,
                ProfilePicture = receiverUser.ProfilePicture
            })
            .Reverse()
            .Skip((int)(page - 1) * 10)
            .Take(10);


        var receivedMessages = receiverUser.ReceivedMessages
            .Where(m => m.SenderId == senderUser.Id)
            .Select(m => new
            {
                m.SenderId,
                m.ReceiverId,
                m.Content,
                m.Date,
                UserName = senderUser.UserName,
                FullName = senderUser.FullName,
                ProfilePicture = senderUser.ProfilePicture
            })
            .Reverse()
            .Skip((int)(page - 1) * 10)
            .Take(10);

        var messages = receivedMessages.Concat(senderMessages).OrderBy(m => m.Date).Reverse().ToList();

        var result = messages.Select(m => new
        {
            m.SenderId,
            m.ReceiverId,
            Content = _encryptServices.DecryptMessage(m.Content),
            m.Date,
            m.UserName,
            m.FullName,
            m.ProfilePicture
        });

        return new OkObjectResult(result);
    }

    public async Task<IActionResult> GetAllLastMessagesGroupByUsers(string token)
    {
        _jwtServices.ValidateToken(token);

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