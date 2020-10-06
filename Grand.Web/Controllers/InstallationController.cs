using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Controllers
{
    [ApiController]
    [Route("installation")]
    public class InstallationController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return View("Components/Installation/Default");
        } 
    }
}