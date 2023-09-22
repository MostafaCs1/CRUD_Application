using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUD_Application.Controllers;

[Route("[Controller]")]
public class CountriesController : Controller
{
    //fields
    private readonly ICountriesService _countryservices;

    //constructor
    public CountriesController(ICountriesService countriesService)
    {
        _countryservices = countriesService;
    }


    //actions
    [HttpGet]
    [Route("[action]")]
    public IActionResult UploadCountriesFromExcelFile()
    {
        return View();
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> UploadCountriesFromExcelFile(IFormFile excelFile)
    {
        //check that user send a file or not
        if (excelFile == null || excelFile.Length == 0)
        {
            ViewData["ErrorMessage"] = "Please choose a xlsx file.";
            return View();
        }
        //check file format
        if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ViewData["ErrorMessage"] = "Please choose a correct file format (.xlsx)";
            return View();
        }

        //add countries into countries list
        int numberOfInsertedCountry = await _countryservices.UploadCountriesFromExcelFile(excelFile);
        ViewData["SuccessMessage"] = $"{numberOfInsertedCountry} country add into countries list.";

        return View();
    }
}
