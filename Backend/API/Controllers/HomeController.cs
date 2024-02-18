using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("/")]
public class HomeController
{
    [HttpGet]
    public IActionResult Index()
    {
        return new JsonResult(new { message = "Welcome to the Backend API" });
    }
}