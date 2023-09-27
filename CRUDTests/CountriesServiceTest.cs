using ServiceContracts.DTO;
using Services;
using ServiceContracts;
using Entities;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;

namespace CRUDTests;

public class CountriesServiceTest
{
    //private fields
    private readonly ICountriesService _countriesService;
    private readonly IFixture _fixture;

    //constructor
    public CountriesServiceTest()
    {
        _fixture = new Fixture();

        //Intial country table
        var countriesInitialData = new List<Country>(){ };

        //Get instance of dbContext builder option
        DbContextOptions optionBiulder = new DbContextOptionsBuilder<ApplicationDbContext>().Options;

        //mocking dbContext
        var dbContextMock = new DbContextMock<ApplicationDbContext>(optionBiulder);

        //add mock country table into mock dbcontext
        dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

        _countriesService = new CountriesService(null);
    }

    #region AddCountry
    //If you supply null Country requet it should return arguemnt null exception
    [Fact]
    public async Task AddCountry_NullCountry()
    {
        //Arrange
        CountryAddRequest? countryAddRequest = null;

        //Act
        Func<Task> action = async() => await _countriesService.AddCountry(countryAddRequest);

        //Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    //If you supply null country name it should return argument exception
    [Fact]
    public async Task AddCountry_NullCountryName()
    {
        //Arrange
        CountryAddRequest? countryAddRequest = _fixture.Build<CountryAddRequest>()
            .With(temp => temp.CountryName, null as string).Create();

        //Act
        Func<Task> action = async () => await _countriesService.AddCountry(countryAddRequest);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    //When we supply a duplicate country name it should return argument exception
    [Fact]
    public async Task AddCountry_DuplicateCountryName()
    {
        //Arrange
        CountryAddRequest? request_1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest? request_2 = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, request_1.CountryName).Create();

        //Act
        Func<Task> action = async () =>
        {
            await _countriesService.AddCountry(request_1);
            await _countriesService.AddCountry(request_2);
        };

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    //If we supply correct country name it should return a correct country respone object
    [Fact]
    public async Task AddCountry_PropCountryRequest()
    {
        //Arrange 
        CountryAddRequest? request = _fixture.Create<CountryAddRequest>();

        //Act
        CountryResponse countryResponse = await _countriesService.AddCountry(request);
        List<CountryResponse> countries = await _countriesService.GetAllCountries();

        //Assert
        countryResponse.CountryID.Should().NotBe(Guid.Empty);
        countries.Should().Contain(countryResponse);
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
        actual_country_list.Should().BeEmpty();
    }

    //When we add some coutry to county list it should return their in the list
    [Fact]
    public async Task GetAllCountries_AddSomeCountry()
    {
        //Arrange
        List<CountryAddRequest> country_add_request_list = _fixture.Create<List<CountryAddRequest>>();

        var expected_country_response = new List<CountryResponse>();

        //Act
        foreach(CountryAddRequest request in country_add_request_list)
        {
            CountryResponse countryResponse = await _countriesService.AddCountry(request);
            expected_country_response.Add(countryResponse);
        }

        List<CountryResponse> actual_country_response = await _countriesService.GetAllCountries();

        //Assert
        actual_country_response.Should().BeEquivalentTo(expected_country_response);
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
        response.Should().BeNull();
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
        response?.Should().BeNull();
    }

    //If you supply true country id it should return proper country response
    [Fact]
    public async Task GetCountryByCountryID_ValidCountryID()
    {
        //Arrange
        CountryAddRequest request = _fixture.Create<CountryAddRequest>();

        //Act
        CountryResponse response_from_add = await _countriesService.AddCountry(request);
        CountryResponse? response_from_get = await _countriesService.GetCountryByCountryID(response_from_add.CountryID);

        //Assert
        response_from_get.Should().Be(response_from_add);
    }

    #endregion
}
