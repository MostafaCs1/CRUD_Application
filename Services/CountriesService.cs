using ServiceContracts;
using ServiceContracts.DTO;
using Entities;

namespace Services
{
    public class CountriesService : ICountriesService
    {
        //private fileds
        private List<Country> _countries;

        //constractor
        public CountriesService()
        {
            _countries = new List<Country>();
        }


        public CountryResponse AddCountry(CountryAddRequest? countryAddRequest)
        {
            //validation : country add request can't be null
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            //validation : country add request's name can't be null
            if(string.IsNullOrEmpty(countryAddRequest.CountryName))
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            //validation : country name can't be duplicate
            if (_countries.Any(country => country.CountryName == countryAddRequest.CountryName))
            {
                throw new ArgumentException("Given country name is already exist.");
            }

            //change country add request to country object
            Country newCountry = countryAddRequest.ToCountry();

            //generate new countryId 
            newCountry.CountryID = Guid.NewGuid();

            //add country to country list
            _countries.Add(newCountry);

            return newCountry.ToCountryResponse();
        }

        public List<CountryResponse> GetAllCountries()
        {
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }
    }
}