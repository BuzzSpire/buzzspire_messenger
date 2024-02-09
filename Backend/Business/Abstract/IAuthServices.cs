using Backend.Entity.DTO.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Business.Abstract;

public interface IAuthServices
{
    public string Login(string email, string password);
    public Task<IActionResult> Register(RegisterRequest registerRequest);
}