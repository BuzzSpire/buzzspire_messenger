using Backend.Business.Abstract;
using Backend.Entity.DTO.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    
    private readonly IAuthServices _authServices;
    
    public AuthController(IAuthServices authServices)
    {
        _authServices = authServices;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        return await _authServices.Register(request);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        return await _authServices.Login(request);
    }
}