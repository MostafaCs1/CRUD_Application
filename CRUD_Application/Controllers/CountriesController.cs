using Microsoft.AspNetCore.Mvc;

namespace CRUD_Application.Controllers;

[Route("[Controller]")]
public class CountriesController : Controller
{
    [HttpGet]
    [Route("[action]")]
    public IActionResult UploadCountriesFromExcelFile()
    {
        return View();
    }

    [HttpPost]
    [Route("[action]")]
    public IActionResult UploadCountriesFromExcelFile(IFormFile formFile)
    {
        return View();
    }
}
