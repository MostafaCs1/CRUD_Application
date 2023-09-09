using System;
using System.Linq;
using ServiceContracts.DTO;
using Services;
using ServiceContracts;

namespace CRUDTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesService _countriesService;

        public CountriesServiceTest(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        #region AddCountry
        //If you supply null Country requet it should return arguemnt null exception
        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = null;

            //Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => 
            {
                //Act
                await _countriesService.AddCountry(countryAddRequest);
            });
        }

        //If you supply null country name it should return argument exception
        [Fact]
        public async Task AddCountry_NullCountryName()
        {
            //Arrange
            CountryAddRequest? countryAddRequest = new CountryAddRequest { CountryName = null };

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _countriesService.AddCountry(countryAddRequest);
            });
        }

        //When we supply a duplicate country name it should return argument exception
        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request_1 = new CountryAddRequest { CountryName = "Iran" };
            CountryAddRequest? request_2 = new CountryAddRequest { CountryName = "Iran" };            

            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                //Act
                await _countriesService.AddCountry(request_1);
                await _countriesService.AddCountry(request_2);
            });

        }

        //If we supply correct country name it should return a correct country respone object
        [Fact]
        public async Task AddCountry_PropCountryRequest()
        {
            //Arrange 
            CountryAddRequest? request = new CountryAddRequest { CountryName = "Iran" };

            //Act
            CountryResponse countryResponse = await _countriesService.AddCountry(request);
            List<CountryResponse> countries = await _countriesService.GetAllCountries();

            //Assert
            Assert.True(countryResponse.CountryID != Guid.Empty);
            Assert.Contains(countryResponse, countries);
        }

        #endregion

        #region GetAllCountries
        //County list should be empty before we add county
        [Fact]
        public async Task GetAllCountries_EmptyList()
        {
            //Act
            List<CountryResponse> actual_country_list = await _countriesService.GetAllCountries();

            //Assert
            Assert.Empty(actual_country_list);
        }

        //When we add some coutry to county list it should return their in the list
        [Fact]
        public async Task GetAllCountries_AddSomeCountry()
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
                CountryResponse countryResponse = await _countriesService.AddCountry(request);
                expected_country_response.Add(countryResponse);
            }

            List<CountryResponse> actual_country_response = await _countriesService.GetAllCountries();

            //Assert
            foreach(CountryResponse countryResponse in expected_country_response)
            {
                Assert.Contains(countryResponse, actual_country_response);
            }
        }
        #endregion

        #region GetCountryByCountryID
        //If you supply null country id it should return null
        [Fact]
        public async Task GetCountryByCountryID_NullCountryID()
        {
            //Arrange
            Guid? countryID = null;

            //Act
            CountryResponse? response = await _countriesService.GetCountryByCountryID(countryID);

            //Assert
            Assert.Null(response);
        }

        //If you supply invalid country id it should return null value
        [Fact]
        public async Task GetCountryByCountryID_InvalidCountryID()
        {
            //Arrange
            Guid countryID = Guid.NewGuid();

            //Act
            CountryResponse? response = await _countriesService.GetCountryByCountryID(countryID);

            //Assert
            Assert.Null(response);
        }

        //If you supply true country id it should return proper country response
        [Fact]
        public async Task GetCountryByCountryID_ValidCountryID()
        {
            //Arrange
            CountryAddRequest request = new CountryAddRequest { CountryName = "Iran" };

            //Act
            CountryResponse response_from_add = await _countriesService.AddCountry(request);
            CountryResponse? response_from_get = await _countriesService.GetCountryByCountryID(response_from_add.CountryID);

            //Assert
            Assert.Equal(response_from_add, response_from_get);
        }

        #endregion
    }
}
