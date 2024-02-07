using Backend.Business.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("/")]
    public class HomeController : ControllerBase
    {
        private IWebSocketServices _webSocketServices;

        public HomeController(IWebSocketServices webSocketServices)
        {
            _webSocketServices = webSocketServices;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return new JsonResult(new { message = "Welcome to Backend" });
        }
    }
}