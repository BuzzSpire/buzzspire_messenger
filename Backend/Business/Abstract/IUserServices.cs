using Backend.Entity.Concrete;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Business.Abstract;

public interface IUserServices
{
   public Task<IActionResult> GetUserByUserNameAsync(string userName, string token);
}