using System;
using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface IPersonsService
    {   
        /// <summary>
        /// Add new person to perons list
        /// </summary>
        /// <param name="request">PersonAddRequet type that you want add into list</param>
        /// <returns>return added person as persons response type.</returns>
        PersonResponse AddPerson(PersonAddRequest? request);

        
        /// <summary>
        /// Get all persons that already exist in list
        /// </summary>
        /// <returns>return all persons as type of list of persons response</returns>
        List<PersonResponse> GetAllPersons();
    }
}
