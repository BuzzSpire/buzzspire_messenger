using Backend.Business.Abstract;
using Backend.Data.Concrete.EF;
using Backend.Entity.Concrete;
using Backend.Entity.DTO.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Business.Concrete;

public class UserServices : IUserServices
{
    private readonly IJwtServices _jwtServices;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IEncryptServices _encryptServices;

    public UserServices(IJwtServices jwtServices, ApplicationDbContext applicationDbContext,
        IEncryptServices encryptServices)
    {
        _jwtServices = jwtServices;
        _applicationDbContext = applicationDbContext;
        _encryptServices = encryptServices;
    }

    public async Task<IActionResult> GetUserByUserNameAsync(string userName, string token)
    {
        _jwtServices.ValidateToken(token);

        User? user = await _applicationDbContext.Users.Where(u => u.UserName == userName).Select(u => new User
        {
            Id = u.Id,
            UserName = u.UserName,
            FullName = u.FullName,
            Bio = u.Bio,
            ProfilePicture = u.ProfilePicture
        }).FirstOrDefaultAsync() ?? null;

        if (user == null)
        {
            return new NotFoundObjectResult(new { error = "User not found" });
        }

        return new OkObjectResult(user);
    }

    public async Task<IActionResult> UploadProfilePictureAsync(IFormFile file, string token)
    {
        _jwtServices.ValidateToken(token);

        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u =>
            u.Id == _jwtServices.GetUserIdFromToken(token));

        if (user == null)
        {
            return new NotFoundObjectResult(new { error = "User not found" });
        }

        if (file.Length > 0)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            user.ProfilePicture = ms.ToArray();

            _applicationDbContext.Users.Update(user);

            try
            {
                await _applicationDbContext.SaveChangesAsync();
                return new OkObjectResult(new { message = "Profile picture uploaded successfully" });
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new { error = e.Message });
            }
        }
        else
        {
            return new BadRequestObjectResult(new { error = "File is empty" });
        }
    }

    public async Task<IActionResult> UpdateUserBasicInfoAsync(UpdateUserBasicInfoRequest request, string token)
    {
        _jwtServices.ValidateToken(token);

        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u =>
            u.Id == _jwtServices.GetUserIdFromToken(token));

        if (user == null)
        {
            return new NotFoundObjectResult(new { error = "User not found" });
        }

        user.FullName = request.FullName;
        user.Bio = request.Bio;

        _applicationDbContext.Users.Update(user);

        try
        {
            await _applicationDbContext.SaveChangesAsync();
            return new OkObjectResult(new { message = "User updated successfully" });
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(new { error = e.Message });
        }
    }

    public async Task<IActionResult> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, string token)
    {
        _jwtServices.ValidateToken(token); 

        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(u =>
            u.Id == _jwtServices.GetUserIdFromToken(token));

        if (user == null)
        {
            return new NotFoundObjectResult(new { error = "User not found" });
        }

        if (user.Password != _encryptServices.EncryptPassword(request.OldPassword) ||
            request.ConfirmPassword != request.NewPassword)
        {
            return new BadRequestObjectResult(new { error = "Invalid password" });
        }

        user.Password = _encryptServices.EncryptPassword(request.NewPassword);
        _applicationDbContext.Users.Update(user);

        try
        {
            await _applicationDbContext.SaveChangesAsync();
            return new OkObjectResult(new { message = "Password updated successfully" });
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(new { error = e.Message });
        }
    }
}