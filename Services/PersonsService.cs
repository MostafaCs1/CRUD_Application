using System;
using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Services.Helper;
using ServiceContracts.Enums;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        //fields
        private readonly PersonDbContext _db;
        private readonly ICountriesService _countryService;

        //constructor
        public PersonsService(PersonDbContext personDbContext, ICountriesService countriesService)
        {
            _db = personDbContext;
            _countryService = countriesService;
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
            _db.Persons.Add(newPerson);
            _db.SaveChanges();

            return ConvetrPersonToPersonResponse(newPerson);
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _db.Persons.ToList().Select(person => ConvetrPersonToPersonResponse(person)).ToList();
        }

        public PersonResponse? GetPersonByPersonID(Guid? personID)
        {
            //validation: personId can't be null
            if (personID == null)
                return null;

            Person? response = _db.Persons.FirstOrDefault(person => person.PersonID == personID);

            //validation" personId can't be invalid
            if (response == null)
                return null;

            return response.ToPersonResponse();
        }

        public List<PersonResponse> GetFiltredPersons(string searchBy, string? searchString)
        {
            List<PersonResponse> allPersons = GetAllPersons();
            List<PersonResponse> matchingPersons = allPersons;
            if (string.IsNullOrEmpty(searchString) || string.IsNullOrEmpty(searchBy))
            {
                return matchingPersons;
            }

            switch (searchBy)
            {
                case nameof(PersonResponse.PersonName):
                    matchingPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.PersonName))
                        ? person.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Email):
                    matchingPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Email))
                        ? person.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Age):
                    matchingPersons = allPersons.Where(person => (person.Age != null)
                        ? person.Age.Equals(searchString) : true).ToList();
                    break;

                case nameof(PersonResponse.Address):
                    matchingPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Address))
                        ? person.Address.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.CountryID):
                    matchingPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Country))
                        ? person.Country.Contains(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.Gender):
                    matchingPersons = allPersons.Where(person => (!string.IsNullOrEmpty(person.Gender))
                        ? person.Gender.Equals(searchString, StringComparison.OrdinalIgnoreCase) : true).ToList();
                    break;

                case nameof(PersonResponse.DateOfBirth):
                    matchingPersons = allPersons.Where(person => (person.DateOfBirth != null)
                        ? person.DateOfBirth.Value.ToString("dd MMMM yyyy").Contains(searchString) : true).ToList();
                    break;
            }

            return matchingPersons;
        }

        public List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder)
        {
            if (sortBy == null)
            {
                return allPersons;
            }

            List<PersonResponse> sortedPersons = (sortBy, sortOrder) switch
            {
                (nameof(PersonResponse.PersonName), SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.PersonName), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(temp => temp.PersonName, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Email), SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Email), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(temp => temp.Email, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Address), SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Address), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(temp => temp.Address, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Country), SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Country), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(temp => temp.Country, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.Age), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.Age).ToList(),
                (nameof(PersonResponse.Age), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.Age).ToList(),

                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.DateOfBirth).ToList(),
                (nameof(PersonResponse.DateOfBirth), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.DateOfBirth).ToList(),

                (nameof(PersonResponse.Gender), SortOrderOptions.ASC) =>
                allPersons.OrderBy(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
                (nameof(PersonResponse.Gender), SortOrderOptions.DESC) =>
                allPersons.OrderByDescending(temp => temp.Gender, StringComparer.OrdinalIgnoreCase).ToList(),

                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.ASC) => allPersons.OrderBy(temp => temp.ReceiveNewsLetters).ToList(),
                (nameof(PersonResponse.ReceiveNewsLetters), SortOrderOptions.DESC) => allPersons.OrderByDescending(temp => temp.ReceiveNewsLetters).ToList(),

                _ => allPersons
            };

            return sortedPersons;
        }

        public PersonResponse UpdatePerson(PersonUpdateRequest? personUpdate)
        {
            //Update request can't be null
            if(personUpdate == null) 
                throw new ArgumentNullException(nameof(personUpdate));

            //Model validation
            ValidationHelper.ModelValidation(personUpdate);

            Person? matchingPerson = _db.Persons.FirstOrDefault(temp => temp.PersonID == personUpdate.PersonID);
            //PersonID should be a valid person Id
            if (matchingPerson == null)
                throw new ArgumentException("Given person Id isn't exist in persons list.");

            //update person details
            matchingPerson.PersonName = personUpdate.PersonName;
            matchingPerson.Email = personUpdate.Email;
            matchingPerson.Address = personUpdate.Address;
            matchingPerson.Gender = personUpdate.Gender.ToString();
            matchingPerson.ReceiveNewsLetters = personUpdate.ReceiveNewsLetters;
            matchingPerson.DateOfBirth = personUpdate.DateOfBirth;
            matchingPerson.CountryID = personUpdate.CountryID;

            //UPDATE Database
            _db.SaveChanges();

            return matchingPerson.ToPersonResponse();
        }

        public bool DeletePerson(Guid? personID)
        {
            //PersonsID can't be null
            if(personID == null)
                throw new ArgumentNullException(nameof(personID));

            Person? person = _db.Persons.FirstOrDefault(person => person.PersonID == personID);
            if(person == null)
                return false;

            _db.Persons.Remove(person);
            _db.SaveChanges();

            return true;
        }
    }
}

