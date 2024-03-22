using Backend.Entity.Concrete;
using Backend.Entity.DTO.User;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Business.Abstract;

public interface IUserServices
{
    public Task<IActionResult> GetUserByUserNameAsync(string userName, string token);
    Task<IActionResult> UploadProfilePictureAsync(IFormFile file, string token);
    Task<IActionResult> UpdateUserBasicInfoAsync(UpdateUserBasicInfoRequest request, string token);
    Task<IActionResult> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, string token);
    Task<IActionResult> IsUserOnlineAsync(string userName, string token);
}