using ServiceContracts;
using ServiceContracts.DTO;
using Microsoft.EntityFrameworkCore;
using Entities;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private fileds
        private readonly PersonDbContext _db;

        //constractor
        public CountriesService(PersonDbContext personDbContext)
        {
            _db = personDbContext;
        }

        //Services
        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            //validation : country add request can't be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //validation : country add request's name can't be null
            if (string.IsNullOrEmpty(countryAddRequest.CountryName))
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //validation : country name can't be duplicate
            if (await _db.Countries.AnyAsync(country => country.CountryName == countryAddRequest.CountryName))
            {
                throw new ArgumentException("Given country name is already exist.");
            }

            //change country add request to country object
            Country newCountry = countryAddRequest.ToCountry();

            //generate new countryId 
            newCountry.CountryID = Guid.NewGuid();

            //add country to country list
            _db.Countries.Add(newCountry);
            await _db.SaveChangesAsync();

            return newCountry.ToCountryResponse();
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            //return all country that exist in countries list as list of response type
            return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();
        }

        public async Task<CountryResponse?> GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? country = await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryID);

            if (country == null)
                return null;

            return country.ToCountryResponse();
        }
    }
}