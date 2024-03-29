using Microsoft.AspNetCore.Mvc;

namespace Backend.Business.Abstract;

public interface IJwtServices
{
    public string GenerateJwt(long userId, string email);
    public long GetUserIdFromToken(string token);
    public string GetEmailFromToken(string token);
    public bool IsTokenValid(string token);
    public IActionResult ValidateToken(string token);
}