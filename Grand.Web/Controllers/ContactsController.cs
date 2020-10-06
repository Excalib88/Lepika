using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Controllers
{
    [ApiController]
    [Route("contacts")]
    public class ContactsController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return View("Components/Contacts/Default");
        } 
    }
}