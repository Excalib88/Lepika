using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Controllers
{
    [ApiController]
    [Route("fitting")]
    public class FittingController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return View("Components/Fitting/Default");
        }
    }
}