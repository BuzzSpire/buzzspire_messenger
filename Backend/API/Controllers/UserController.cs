using Backend.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController: ControllerBase{
    
    private readonly IUserServices _userServices;
    

    public UserController(IUserServices userServices)
    {
        _userServices = userServices;
    }
    
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
    

}