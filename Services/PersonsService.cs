using System;
using ServiceContracts;
using ServiceContracts.DTO;

namespace Services
{
    public class PersonsService : IPersonsService
    {
        public PersonResponse AddPerson(PersonAddRequest? request)
        {
            throw new NotImplementedException();
        }

        public List<PersonResponse> GetAllPersons()
        {
            throw new NotImplementedException();
        }
    }
}
