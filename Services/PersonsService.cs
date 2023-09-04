using System;
using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using System.ComponentModel.DataAnnotations;
using Services.Helper;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        //fields
        private List<Person> _persones;

        //constructor
        public PersonsService()
        {
            _persones = new List<Person>();
        }

        //services
        public PersonResponse AddPerson(PersonAddRequest? request)
        {            
            // PersonsAddrequest can't be null
            if(request == null) 
                throw new ArgumentNullException(nameof(request));

            // Model validation
            ValidationHelper.ModelValidation(request);

            //Model validation
            ValidationContext validationContext = new ValidationContext(request);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults);
            if (!isValid)
            {
                throw new ArgumentException(validationResults.FirstOrDefault()?.ErrorMessage);
            }

            //generate personId
            Guid personID = Guid.NewGuid();

            //create person object
            Person newPerson = request.ToPerson();
            newPerson.PersonID = personID;

            //add person to person list
            _persones.Add(newPerson);

            return newPerson.ToPersonResponse();
        }

        public List<PersonResponse> GetAllPersons()
        {
            return _persones.Select(person => person.ToPersonResponse()).ToList();
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
    }
}
