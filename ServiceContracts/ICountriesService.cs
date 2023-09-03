using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface ICountriesService
    {
        /// <summary>
        /// Add given country object to coutries
        /// </summary>
        /// <param name="countryAddRequest">country object to add</param>
        /// <returns>return corresponding country object as country response</returns>
        CountryResponse AddCountry(CountryAddRequest? countryAddRequest);
    }
}