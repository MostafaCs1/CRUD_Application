using RepositoryContracts;
using Entities;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class PersonsRepository : IPersonsRepository
{
    //private fields
    private readonly ApplicationDbContext _db;

    //constructor
    public PersonsRepository(ApplicationDbContext dbContext)
    {
        _db = dbContext;
    }

    //services
    public async Task<Person> AddPerson(Person person)
    {
        _db.Persons.Add(person);
        await _db.SaveChangesAsync();

        return person;
    }

    public async Task<bool> DeletePersonByPersonID(Guid personID)
    {
        _db.Persons.RemoveRange(_db.Persons.Where(person => person.PersonID == personID));
        int rowDeleted =  await _db.SaveChangesAsync();

        return rowDeleted > 0;
    }

    public async Task<List<Person>> GetAllPersons()
    {
        return await _db.Persons.Include("Country").ToListAsync();
    }

    public async Task<List<Person>> GetFiltredPersons(Expression<Func<Person, bool>> func)
    {
        return await _db.Persons.Include("Country").Where(func).ToListAsync();
    }

    public async Task<Person?> GetPersonByPersonID(Guid personID)
    {
        return await _db.Persons.Include("Country").FirstOrDefaultAsync(person => person.PersonID == personID);
    }

    public async Task<Person> UpdatePerson(Person person)
    {
        //get matching person for update
        Person? matchingPerson = await _db.Persons.FirstOrDefaultAsync(temp => temp.PersonID == person.PersonID);

        if (matchingPerson == null)
        {
            return person;
        }

        //update matching person's details
        matchingPerson.PersonName = person.PersonName;
        matchingPerson.Gender = person.Gender;
        matchingPerson.Email = person.Email;
        matchingPerson.Address = person.Address;
        matchingPerson.DateOfBirth = person.DateOfBirth;
        matchingPerson.CountryID = person.CountryID;
        matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;

        await _db.SaveChangesAsync();

        return matchingPerson;
    }

}