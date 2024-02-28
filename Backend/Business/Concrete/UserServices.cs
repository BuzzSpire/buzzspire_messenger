using Backend.Business.Abstract;
using Backend.Data.Concrete.EF;
using Backend.Entity.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Business.Concrete;

public class UserServices : IUserServices
{
    private IJwtServices _jwtServices;
    private readonly ApplicationDbContext _applicationDbContext;

    public UserServices(IJwtServices jwtServices, ApplicationDbContext applicationDbContext)
    {
        _jwtServices = jwtServices;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<IActionResult> GetUserByUserNameAsync(string userName, string token)
    {
        if (!_jwtServices.IsTokenValid(token))
        {
            return new UnauthorizedObjectResult(new { error = "Unauthorized" });
        }

        User? user = await _applicationDbContext.Users.Where(u => u.UserName == userName).Select(u => new User
        {
            Id = u.Id,
            UserName = u.UserName,
            FullName = u.FullName,
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
        if (!_jwtServices.IsTokenValid(token))
        {
            return new UnauthorizedObjectResult(new { error = "Unauthorized" });
        }

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

        return new OkObjectResult(new { message = "Profile picture uploaded successfully" });
    }
}