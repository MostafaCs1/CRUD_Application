using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUD_Application.Controllers
{    
    [Route("[controller]")]
    public class PersonController : Controller
    {
        //private fields
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countryService;

        //constructor
        public PersonController(IPersonsService personsService, ICountriesService countriesService)
        {
            _personService = personsService;
            _countryService = countriesService;
        }


        //actions

        [HttpGet]
        [Route("/")]
        [Route("[action]")]
        public IActionResult Index(string searchBy, string? searchString)
        {
            ViewData["searchList"] = new Dictionary<string, string>()
            {
                {"Person Name", nameof(PersonResponse.PersonName) },
                {"Email", nameof(PersonResponse.Email) },
                {"Date of Birth", nameof(PersonResponse.DateOfBirth) },
                {"Address", nameof(PersonResponse.Address) },
                {"Gender", nameof(PersonResponse.Gender) },
                {"Country", nameof(PersonResponse.CountryID)},
            };

            ViewBag.searchBy = searchBy;
            ViewBag.searchString = searchString;
            
            List<PersonResponse> persons = _personService.GetFiltredPersons(searchBy, searchString);
            
            return View(persons);
        }
    }
}
