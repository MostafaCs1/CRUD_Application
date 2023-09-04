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
            throw new NotImplementedException();
        }
    }
}
