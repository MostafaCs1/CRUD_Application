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
        public CountriesService(bool initialize = true)
        {
            _countries = new List<Country>();

            if (initialize)
            {
                _countries.AddRange(new List<Country>() {
                    new Country() {  CountryID = Guid.Parse("000C76EB-62E9-4465-96D1-2C41FDB64C3B"), CountryName = "USA" },

                    new Country() { CountryID = Guid.Parse("32DA506B-3EBA-48A4-BD86-5F93A2E19E3F"), CountryName = "Canada" },

                    new Country() { CountryID = Guid.Parse("DF7C89CE-3341-4246-84AE-E01AB7BA476E"), CountryName = "Iran" },

                    new Country() { CountryID = Guid.Parse("15889048-AF93-412C-B8F3-22103E943A6D"), CountryName = "India" },

                    new Country() { CountryID = Guid.Parse("80DF255C-EFE7-49E5-A7F9-C35D7C701CAB"), CountryName = "Australia" }
                   });
            }
        }


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
            //return all country that exist in countries list as list of response type
            return _countries.Select(country => country.ToCountryResponse()).ToList();
        }

        public CountryResponse? GetCountryByCountryID(Guid? countryID)
        {
            if (countryID == null)
                return null;

            Country? country = _countries.FirstOrDefault(temp => temp.CountryID == countryID);

            if (country == null)
                return null;

            return country.ToCountryResponse();
        }
    }
}