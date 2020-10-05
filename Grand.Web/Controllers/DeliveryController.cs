using System.Threading.Tasks;
using Grand.Web.Models.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Controllers
{
    [ApiController]
    [Route("delivery")]
    public class DeliveryController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return View("Components/Delivery/Default", new DeliveryModel());
        }
    }
}