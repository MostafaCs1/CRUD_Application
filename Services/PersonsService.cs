using System;
using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services.Helper;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        //fields
        private List<Person> _persones;
        private readonly ICountriesService _countryService;

        //constructor
        public PersonsService()
        {
            _persones = new List<Person>();
            _countryService = new CountriesService();
        }

        //methods
        private PersonResponse ConvetrPersonToPersonResponse(Person person)
        {
            PersonResponse response = person.ToPersonResponse();
            response.Country = _countryService.GetCountryByCountryID(person.CountryID)?.CountryName;
            return response;
        }

        //services
        public PersonResponse AddPerson(PersonAddRequest? request)
        {
            // PersonsAddrequest can't be null
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Model validation
            ValidationHelper.ModelValidation(request);

            //generate personId
            Guid personID = Guid.NewGuid();

            //create person object
            Person newPerson = request.ToPerson();
            newPerson.PersonID = personID;

            //add person to person list
            _persones.Add(newPerson);

            return ConvetrPersonToPersonResponse(newPerson);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persones.Select(person => ConvetrPersonToPersonResponse(person)).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            //validation: personId can't be null
            if (personID == null)
                return null;

            Person? response = _persones.FirstOrDefault(person => person.PersonID == personID);

            //validation" personId can't be invalid
            if (response == null)
                return null;

            return response.ToPersonResponse();
        }

        public List<PersonResponse> GetFiltredPersons(string searchBy, string? searchString)
        {
            throw new NotImplementedException();
        }
    }
}
