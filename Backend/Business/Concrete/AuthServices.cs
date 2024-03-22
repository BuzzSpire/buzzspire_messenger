using System.Collections;
using System.Runtime.InteropServices.JavaScript;
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
    private readonly IEncryptServices _encryptServices;

    public AuthServices(IJwtServices jwtServices, ApplicationDbContext applicationDbContext,
        IEncryptServices encryptServices)
    {
        _jwtServices = jwtServices;
        _applicationDbContext = applicationDbContext;
        _encryptServices = encryptServices;
    }


    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u =>
            u.UserName == request.UserName && u.Password == _encryptServices.EncryptPassword(request.Password));
        if (user == null)
        {
            return new BadRequestObjectResult(new { error = "UserName or Password wrong" });
        }
        else
        {
            return new OkObjectResult(new { token = _jwtServices.GenerateJwt(user.Id, user.UserName) });
        }
    }

    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
        if (user != null)
        {
            return new BadRequestObjectResult(new { error = "User already exists" });
        }

        if (request.UserName.Contains($" "))
        {
            return new BadRequestObjectResult(new { error = "Username can not contain space" });
        }

        if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password) ||
            string.IsNullOrEmpty(request.FullName))
        {
            return new BadRequestObjectResult(new { error = "Username, Password and Fullname can not be empty" });
        }

        request.Password = _encryptServices.EncryptPassword(request.Password);

        _applicationDbContext.Users.Add(new User
        {
            FullName = request.FullName,
            UserName = request.UserName,
            Password = request.Password,
            Bio = "",
            ProfilePicture = new Byte[0],
            ReceivedMessages = new List<Message>(),
            SendMessages = new List<Message>()
        });

        try
        {
            await _applicationDbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }

        user = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
        if (user == null)
        {
            return new BadRequestObjectResult(new { error = "User not found" });
        }

        return new OkObjectResult(new { token = _jwtServices.GenerateJwt(user.Id, user.UserName) });
    }

    public async Task<IActionResult> Auth(string token)
    {
        _jwtServices.ValidateToken(token);

        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u =>
            u.Id == _jwtServices.GetUserIdFromToken(token));

        if (user == null)
        {
            return new NotFoundObjectResult(new { error = "User not found" });
        }

        return new OkObjectResult(new { isvalidated = true });
    }
}