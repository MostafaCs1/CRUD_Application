using System;
using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of CountriesService methods
    /// </summary>
    public class CountryResponse
    {
        public Guid? CountryID { get; set; }
        public string? CountryName { get; set; }


        //check that both object are same then it return true, otherwise return false
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(CountryResponse))
            {
                return false;
            }

            CountryResponse response = (CountryResponse) obj;
            return this.CountryID == response.CountryID && this.CountryName == response.CountryName;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public static class CountryExtention
    {
        /// <summary>
        /// This is an extention method that change country object to country response DTO
        /// </summary>
        /// <returns>return country response object</returns>
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse()
            {
                CountryID = country.CountryID,
                CountryName = country.CountryName
            };
        }
    }
}
