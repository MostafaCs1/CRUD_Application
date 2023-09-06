using System;
using ServiceContracts.DTO;
using ServiceContracts.Enums;

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

        /// <summary>
        ///  search persons list by person id
        /// </summary>
        /// <param name="personID">Person Id that you want to search</param>
        /// <returns>Returns matching person object</returns>
        PersonResponse? GetPersonByPersonID(Guid? personID);

        /// <summary>
        /// Returns all person objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all matching persons based on the given search field and search string</returns>
        List<PersonResponse> GetFiltredPersons(string searchBy, string? searchString);

        /// <summary>
        /// Returns sorted list of persons
        /// </summary>
        /// <param name="allPersons">Represents list of persons to sort</param>
        /// <param name="sortBy">Name of the property (key), based on which the persons should be sorted</param>
        /// <param name="sortOrder">ASC or DESC</param>
        /// <returns>Returns sorted persons as PersonResponse list</returns>
        List<PersonResponse> GetSortedPersons(List<PersonResponse> allPersons, string sortBy, SortOrderOptions sortOrder);

        /// <summary>
        /// Updates the specified person details based on the given person ID
        /// </summary>
        /// <param name="personUpdateRequest">Person details to update, including person id</param>
        /// <returns>Returns the person response object after updation</returns>
        PersonResponse UpdatePerson(PersonUpdateRequest? personUpdate);

        /// <summary>
        /// Delete person from list with given PersonID
        /// </summary>
        /// <param name="personID">PersonId to delete</param>
        /// <returns>return true if remove person is successful, otherwise false</returns>
        bool DeletePerson(Guid? personID);
    }
}
