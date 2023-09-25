using RepositoryContracts;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repositories;

public class CountriesRepository : ICountriesRepository
{
    //private fields
    private readonly ApplicationDbContext _db;

    //constructor
    public CountriesRepository(ApplicationDbContext dbContext)
    {
        _db = dbContext;
    }

    //services
    public async Task<Country> AddCountry(Country country)
    {
        _db.Countries.Add(country);
        await _db.SaveChangesAsync();

        return country;
    }

    public async Task<List<Country>> GetAllCountry()
    {
        return await _db.Countries.ToListAsync();
    }

    public async Task<Country?> GetCountryByCountryID(Guid countryID)
    {
        return await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryID);
    }

    public async Task<Country?> GetCountryByCountryName(string countryName)
    {
        return await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryName == countryName);
    }
}
