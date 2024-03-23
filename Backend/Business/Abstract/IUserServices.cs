using Backend.Entity.Concrete;
using Backend.Entity.DTO.User;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Business.Abstract;

public interface IUserServices
{
    public Task<IActionResult> GetUserByUserNameAsync(string userName, string token);
    public Task<IActionResult> UploadProfilePictureAsync(IFormFile file, string token);
    public Task<IActionResult> UpdateUserBasicInfoAsync(UpdateUserBasicInfoRequest request, string token);
    public Task<IActionResult> UpdateUserPasswordAsync(UpdateUserPasswordRequest request, string token);
    public IActionResult IsUserOnlineAsync(long userId, string token);
}