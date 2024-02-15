using Backend.Business.Abstract;
using Backend.Data.Concrete.EF;
using Backend.Entity.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend.Business.Concrete;

public class UserServices: IUserServices
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
            return new UnauthorizedObjectResult(new {error = "Unauthorized"});
        }
        
        User? user = await _applicationDbContext.Users.Where(u => u.UserName == userName).Select(u => new User
        {
            Id = u.Id,
            UserName = u.UserName,
            FullName = u.FullName
        }).FirstOrDefaultAsync() ?? null;
        
        if (user == null)
        {
            return new NotFoundObjectResult(new  {error = "User not found"});
        }
        
        return new OkObjectResult(user);
    }
}