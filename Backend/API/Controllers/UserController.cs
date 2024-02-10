using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController: ControllerBase{

    [HttpGet("get")]
    public async Task<ActionResult> Get()
    {
        return Ok("Get");
    }

    [HttpPost("update")]
    public async Task<ActionResult> Update()
    {
        return Ok("Update");
    }
    
    [HttpPost("delete")]
    public async Task<ActionResult> Delete()
    {
        return Ok("Delete");
    }
}