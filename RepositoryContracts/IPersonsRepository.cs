using Entities;
using System.Linq.Expressions;

namespace RepositoryContracts;

/// <summary>
/// Represents data access logic for managing Person entity
/// </summary>
public interface IPersonsRepository
{
    /// <summary>
    /// Add given person object into database
    /// </summary>
    /// <param name="person">person object to add</param>
    /// <returns>Added person</returns>
    Task<Person> AddPerson(Person person);

    /// <summary>
    /// Return all persons that exist in persons table
    /// </summary>
    /// <returns>List of persons</returns>
    Task<List<Person>> GetAllPersons();

    /// <summary>
    /// Returns a person object based on the given person id
    /// </summary>
    /// <param name="personID">PersonID to search</param>
    /// <returns>If person already exist in persons table return it; otherwise return null</returns>
    Task<Person?> GetPersonByPersonID(Guid personID);

    /// <summary>
    /// Returns all person objects based on the given expression
    /// </summary>
    /// <param name="func">LINQ expression to check</param>
    /// <returns>All matching persons with given condition</returns>
    Task<List<Person>> GetFiltredPersons(Expression<Predicate<Person>> func);

    /// <summary>
    /// Deletes a person object based on the person id
    /// </summary>
    /// <param name="personID">Person ID (guid) to search</param>
    /// <returns>Returns true, if the deletion is successful; otherwise false</returns>
    Task<Person> DeletePersonByPersonID(Guid personID);

    /// <summary>
    /// Updates a person object (person name and other details) based on the given person id
    /// </summary>
    /// <param name="person">Person object to update</param>
    /// <returns>Returns the updated person object</returns>
    Task<Person> UpdatePerson(Person person);
}