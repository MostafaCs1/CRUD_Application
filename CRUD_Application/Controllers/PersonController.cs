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

        [Route("/")]
        [Route("[action]")]
        public IActionResult Index()
        {
            List<PersonResponse> persons = _personService.GetAllPersons();
            
            return View(persons);
        }
    }
}
