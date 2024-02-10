using Backend.Entity.DTO.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Business.Abstract;

public interface IAuthServices
{
    public Task<IActionResult> Login(LoginRequest request);
    public Task<IActionResult> Register(RegisterRequest registerRequest);
    public Task<IActionResult> Auth(string token);
}