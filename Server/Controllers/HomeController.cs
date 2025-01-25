using Microsoft.AspNetCore.Mvc;

namespace Prog.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("check check");
        }
    }
}