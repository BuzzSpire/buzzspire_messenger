using Backend.Business.Abstract;
using Backend.Entity.DTO.User;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserServices _userServices;


    public UserController(IUserServices userServices)
    {
        _userServices = userServices;
    }

    // get user by username
    [HttpGet("search/{username}")]
    public async Task<IActionResult> GetUserByIdAsync([FromRoute] string username, [FromHeader] string token)
    {
        return await _userServices.GetUserByUserNameAsync(username, token);
    }

    // upload profile picture
    [HttpPost("uploadProfilePicture")]
    public async Task<IActionResult> UploadProfilePictureAsync([FromForm] IFormFile file, [FromHeader] string token)
    {
        return await _userServices.UploadProfilePictureAsync(file, token);
    }

    // update user basic info
    [HttpPut("updateBasicInfo")]
    public async Task<IActionResult> UpdateBasicInfoAsync([FromBody] UpdateUserBasicInfoRequest request,
        [FromHeader] string token)
    {
        return await _userServices.UpdateUserBasicInfoAsync(request, token);
    }

    // update user password
    [HttpPut("updatePassword")]
    public async Task<IActionResult> UpdatePasswordAsync([FromBody] UpdateUserPasswordRequest request,
        [FromHeader] string token)
    {
        return await _userServices.UpdateUserPasswordAsync(request, token);
    }

    // is user online
    [HttpGet("isOnline/{id}")]
    public IActionResult IsUserOnlineAsync([FromRoute] long id, [FromHeader] string token)
    {
        return _userServices.IsUserOnlineAsync(id, token);
    }
}