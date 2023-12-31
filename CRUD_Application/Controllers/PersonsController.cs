﻿using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRUD_Application.Controllers
{    
    [Route("[controller]")]
    public class PersonsController : Controller
    {
        //private fields
        private readonly IPersonsService _personService;
        private readonly ICountriesService _countryService;
        private readonly ILogger<PersonsController> _logger;

        //constructor
        public PersonsController(IPersonsService personsService, ICountriesService countriesService, ILogger<PersonsController> logger)
        {
            _personService = personsService;
            _countryService = countriesService;
            _logger = logger;
        }

        //actions

        [HttpGet]
        [Route("/")]
        [Route("[action]")]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy, SortOrderOptions sortOrder)
        {
            _logger.LogInformation("Index action method in PersonsController");
            _logger.LogDebug($"searchBy :{searchBy}, searchString :{searchString}, sortBy :{sortBy}, sortOrder:{sortOrder}");

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
            ViewBag.CurrentSortOrder = sortOrder;
            ViewBag.CurrentSortBy = sortBy;
            
            List<PersonResponse> persons = await _personService.GetFiltredPersons(searchBy, searchString);
            List<PersonResponse> sortedPersons = _personService.GetSortedPersons(persons, sortBy, sortOrder);
            
            return View(sortedPersons);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countryService.GetAllCountries();
            ViewData["countries"] = countries.Select(temp => new SelectListItem { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create(PersonAddRequest request)
        {
            if(!ModelState.IsValid)
            {
                //get all errors
                ViewData["errors"] =  ModelState.Values.SelectMany(value => value.Errors).Select( error => error.ErrorMessage).ToList();

                //get countries list
                List<CountryResponse> countries = await _countryService.GetAllCountries();
                ViewData["countries"] = countries.Select(temp => new SelectListItem { Text = temp.CountryName, Value = temp.CountryID.ToString() });

                return View();
            }
            await _personService.AddPerson(request);
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? matchingPerson = await _personService.GetPersonByPersonID(personID);
            if(matchingPerson == null)
            {
                return RedirectToAction("Index", "Person");
            }
            //get countries list
            List<CountryResponse> countries = await _countryService.GetAllCountries();
            ViewData["countries"] = countries.Select(temp => new SelectListItem { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            PersonUpdateRequest personUpdateRequest = matchingPerson.ToPersonUpdateRequest();
            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Edit(PersonUpdateRequest updateRequest)
        {
            if (!ModelState.IsValid)
            {
                ViewData["errors"] = ModelState.Values.SelectMany(value => value.Errors).Select(error => error.ErrorMessage).ToList();

                //get countries list
                List<CountryResponse> countries = await _countryService.GetAllCountries();
                ViewData["countries"] = countries.Select(temp => new SelectListItem { Text = temp.CountryName, Value = temp.CountryID.ToString() });

                return View();
            }
            await _personService.UpdatePerson(updateRequest);
            return RedirectToAction("Index", "Persons");
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personObject = await _personService.GetPersonByPersonID(personID);
            if (personObject == null)
                return RedirectToAction("Index", "Person");
            
            return View(personObject);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Delete(PersonResponse response)
        {
            PersonResponse? personObject = await _personService.GetPersonByPersonID(response.PersonID);
            if(personObject == null)
            {
                return RedirectToAction("Index", "Person");
            }

            //check that person is deleted or not
            bool isDeleted = await _personService.DeletePerson(personObject.PersonID);
            if(!isDeleted)
            {
                return View(personObject);
            }

            return RedirectToAction("Index", "Persons");
        }

        [Route("PersonsPDF")]
        public async Task<IActionResult> PersonsPDF()
        {
            List<PersonResponse> persons = await _personService.GetAllPersons();
            return new ViewAsPdf("PersonsPDF", persons)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Bottom = 20, Top = 20, Left = 20, Right = 20},
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Route("PersonsCSV")]
        public async Task<IActionResult> PersonsCSV()
        {
            MemoryStream personsCsv = await _personService.GetPersonsCSV();

            return File(personsCsv, "Application/octet-stream", "Persons.csv");
        }

        [Route("PersonsExcel")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream personsExcel = await _personService.GetPersonExcel();

            return File(personsExcel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Persons.xlsx");
        }

    }
}
