using System;
using System.Collections.Generic;
using System.Linq;
using ServiceContracts.DTO;
using Services;
using ServiceContracts;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest()
        {
            _countriesService = new CountriesService();
        }

        #region AddCountry
        //If you supply null Country requet it should return arguemnt null exception
        [Fact]
        public void AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() => 
            {
                //Act
                _countriesService.AddCountry(countryAddRequest);
            });
        }

        //If you supply null country name it should return argument exception
        [Fact]
        public void AddCountry_NullCountryName()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest { CountryName = null };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(countryAddRequest);
            });
        }

        //When we supply a duplicate country name it should return argument exception
        [Fact]
        public void AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request_1 = new CountryAddRequest { CountryName = "Iran" };
            CountryAddRequest? request_2 = new CountryAddRequest { CountryName = "Iran" };            

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _countriesService.AddCountry(request_1);
                _countriesService.AddCountry(request_2);
            });

        }

        //If we supply correct country name it should return a correct country respone object
        [Fact]
        public void AddCountry_PropCountryRequest()
        {
            //Arrange 
            CountryAddRequest? request = new CountryAddRequest { CountryName = "Iran" };

            //Act
            CountryResponse countryResponse = _countriesService.AddCountry(request);
            List<CountryResponse> countries = _countriesService.GetAllCountries();

            //Assert
            Assert.True(countryResponse.CountryID != Guid.Empty);
            Assert.Contains(countryResponse, countries);
        }

        #endregion

        #region GetAllCountries
        //County list should be empty before we add county
        [Fact]
        public void GetAllCountries_EmptyList()
        {
            //Act
            List<CountryResponse> actual_country_list = _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual_country_list);
        }

        //When we add some coutry to county list it should return their in the list
        [Fact]
        public void GetAllCountries_AddSomeCountry()
        {
            //Arrange
            List<CountryAddRequest> country_add_request_list = new List<CountryAddRequest>
            {
                new CountryAddRequest { CountryName = "Iran" },
                new CountryAddRequest { CountryName = "USA" },
                new CountryAddRequest { CountryName = "India" }
            };

            List<CountryResponse> expected_country_response = new List<CountryResponse>();

            //Act
            foreach(CountryAddRequest request in country_add_request_list)
            {
                CountryResponse countryResponse = _countriesService.AddCountry(request);
                expected_country_response.Add(countryResponse);
            }

            List<CountryResponse> actual_country_response = _countriesService.GetAllCountries();

            //Assert
            foreach(CountryResponse countryResponse in expected_country_response)
            {
                Assert.Contains(countryResponse, actual_country_response);
            }
        }
        #endregion

    }
}
