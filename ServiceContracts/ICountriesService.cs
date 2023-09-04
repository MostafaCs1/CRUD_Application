﻿using ServiceContracts.DTO;

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

        /// <summary>
        /// Get all countries in list of countries
        /// </summary>
        /// <returns>return all counties as list of country response</returns>
        List<CountryResponse> GetAllCountries();

        /// <summary>
        /// Get country object by country name
        /// </summary>
        /// <param name="countryID">CountryID (guid) to search</param>
        /// <returns>Matching country as CountryResponse object</returns>
        CountryResponse? GetCountryByCountryID(Guid? countryID);
    }
}