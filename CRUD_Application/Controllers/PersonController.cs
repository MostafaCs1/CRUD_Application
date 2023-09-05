using Microsoft.AspNetCore.Mvc;

namespace CRUD_Application.Controllers
{
    [Route("[controller]")]
    public class PersonController : Controller
    {
        [Route("/")]
        [Route("[action]")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
