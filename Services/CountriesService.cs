using ServiceContracts;
using ServiceContracts.DTO;
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
        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
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
            if (_db.Countries.Any(country => country.CountryName == countryAddRequest.CountryName))
            {
                throw new ArgumentException("Given country name is already exist.");
            }

            //change country add request to country object
            Country newCountry = countryAddRequest.ToCountry();

            //generate new countryId 
            newCountry.CountryID = Guid.NewGuid();

            //add country to country list
            _db.Countries.Add(newCountry);
            _db.SaveChanges();

            return newCountry.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            //return all country that exist in countries list as list of response type
            return _db.Countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? country = _db.Countries.FirstOrDefault(temp => temp.CountryID == countryID);

            if (country == null)
                return null;

            return country.ToCountryResponse();
        }
    }
}