using System;
using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using System.ComponentModel.DataAnnotations;

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
            // validation : PersonsAddrequest can't be null
            if(request == null) 
                throw new ArgumentNullException(nameof(request));

            // validation : Persons name can't be null
            if(request.PersonName == null)
                throw new ArgumentException(nameof(request.PersonName));

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
