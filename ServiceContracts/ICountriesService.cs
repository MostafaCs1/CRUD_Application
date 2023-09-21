using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts;
public interface ICountriesService
{
    /// <summary>
    /// Add given country object to coutries
    /// </summary>
    /// <param name="countryAddRequest">country object to add</param>
    /// <returns>return corresponding country object as country response</returns>
    Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

    /// <summary>
    /// Get all countries in list of countries
    /// </summary>
    /// <returns>return all counties as list of country response</returns>
    Task<List<CountryResponse>> GetAllCountries();

    /// <summary>
    /// Get country object by country name
    /// </summary>
    /// <param name="countryID">CountryID (guid) to search</param>
    /// <returns>Matching country as CountryResponse object</returns>
    Task<CountryResponse?> GetCountryByCountryID(Guid? countryID);

    /// <summary>
    /// Upload countries from excel file into database
    /// </summary>
    /// <param name="formFile">Excel file with the list of countries</param>
    /// <returns>Return number of countries that added</returns>
    Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
}