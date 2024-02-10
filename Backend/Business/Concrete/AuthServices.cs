using Backend.Business.Abstract;
using Backend.Data.Concrete.EF;
using Backend.Entity.Concrete;
using Backend.Entity.DTO.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Business.Concrete;

public class AuthServices : IAuthServices
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IJwtServices _jwtServices;

    public AuthServices(IJwtServices jwtServices, ApplicationDbContext applicationDbContext)
    {
        _jwtServices = jwtServices;
        _applicationDbContext = applicationDbContext;
    }


    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u =>
            u.UserName == request.UserName && u.Password == _jwtServices.EncryptPassword(request.Password));
        if (user == null)
        {
            return new BadRequestObjectResult(new { error = "UserName or Password wrong" });
        }
        else
        {
            return new OkObjectResult(new { token = _jwtServices.GenerateJwt(user.Id, user.UserName) });
        }
    }

    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserName == registerRequest.UserName);
        if (user != null)
        {
            return new BadRequestObjectResult(new { error = "User already exists" });
        }

        if (string.IsNullOrEmpty(registerRequest.UserName) || string.IsNullOrEmpty(registerRequest.Password) ||
            string.IsNullOrEmpty(registerRequest.FullName))
        {
            return new BadRequestObjectResult(new { error = "Username, Password and Fullname can not be empty" });
        }

        registerRequest.Password = _jwtServices.EncryptPassword(registerRequest.Password);

        _applicationDbContext.Users.Add(new User
        {
            FullName = registerRequest.FullName,
            UserName = registerRequest.UserName,
            Password = registerRequest.Password,
            Bio = "",
        });

        try
        {
            _applicationDbContext.SaveChanges();
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }

        user = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserName == registerRequest.UserName);
        if (user == null)
        {
            return new BadRequestObjectResult(new { error = "User not found" });
        }

        return new OkObjectResult(new { token = _jwtServices.GenerateJwt(user.Id, user.UserName) });
    }
}