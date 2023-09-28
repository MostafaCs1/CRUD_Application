using ServiceContracts.DTO;
using Services;
using ServiceContracts;
using AutoFixture;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using Entities;

namespace CRUDTests;

public class CountriesServiceTest
{
    //private fields
    private readonly ICountriesService _countriesService;

    private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
    private readonly ICountriesRepository _countriesRepository;
    private readonly IFixture _fixture;

    //constructor
    public CountriesServiceTest()
    {
        _fixture = new Fixture();

        //mocking repository
        _countriesRepositoryMock = new Mock<ICountriesRepository>();
        _countriesRepository = _countriesRepositoryMock.Object;

        _countriesService = new CountriesService(_countriesRepository);
    }

    #region AddCountry
    //If you supply null Country requet it should return arguemnt null exception
    [Fact]
    public async Task AddCountry_NullCountry_ToBeArgumentNullException()
    {
        //Arrange
        CountryAddRequest? countryAddRequest = null;

        //Act
        Func<Task> action = async () => await _countriesService.AddCountry(countryAddRequest);

        //Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    //If you supply null country name it should return argument exception
    [Fact]
    public async Task AddCountry_NullCountryName_ToBeArgumentException()
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
    public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
    {
        //Arrange
        CountryAddRequest? request_1 = _fixture.Create<CountryAddRequest>();
        CountryAddRequest? request_2 = _fixture.Build<CountryAddRequest>().With(temp => temp.CountryName, request_1.CountryName).Create();

        Country country_1 = request_1.ToCountry();
        Country country_2 = request_2.ToCountry();

        _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country_1);
        _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(null as Country);

        //Act
        await _countriesService.AddCountry(request_1);

        Func<Task> action = async () =>
        {
            _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country_2);
            _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(country_1);
            await _countriesService.AddCountry(request_2);
        };

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    //If we supply correct country name it should return a correct country respone object
    [Fact]
    public async Task AddCountry_PropCountryRequest_ToBeSuccessful()
    {
        //Arrange 
        CountryAddRequest? country_add_request = _fixture.Create<CountryAddRequest>();
        Country country = country_add_request.ToCountry();
        CountryResponse expected_country_response = country.ToCountryResponse();

        _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).ReturnsAsync(country);
        _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryName(It.IsAny<string>())).ReturnsAsync(null as Country);
        //Act
        CountryResponse country_response_from_add = await _countriesService.AddCountry(country_add_request);
        expected_country_response.CountryID = country_response_from_add.CountryID;

        //Assert
        country_response_from_add.CountryID.Should().NotBe(Guid.Empty);
        country_response_from_add.Should().BeEquivalentTo(expected_country_response);
    }

    #endregion


    #region GetAllCountries
    //County list should be empty before we add county
    [Fact]
    public async Task GetAllCountries_ToBeEmptyList()
    {
        //Arrange
        _countriesRepositoryMock.Setup(temp => temp.GetAllCountry()).ReturnsAsync(new List<Country>());

        //Act
        List<CountryResponse> actual_country_list = await _countriesService.GetAllCountries();

        //Assert
        actual_country_list.Should().BeEmpty();
    }

    //When we add some coutry to county list it should return their in the list
    [Fact]
    public async Task GetAllCountries_WithSomeCountry_ToBeSuccessful()
    {
        //Arrange
        List<Country> countries = new List<Country>()
        {
            _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create(),
            _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create(),
            _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create()
        };

        List<CountryResponse> expected_countries_response = countries.Select(country => country.ToCountryResponse()).ToList();
        _countriesRepositoryMock.Setup(temp => temp.GetAllCountry()).ReturnsAsync(countries);

        //Act
        List<CountryResponse> actual_country_response = await _countriesService.GetAllCountries();

        //Assert
        actual_country_response.Should().BeEquivalentTo(expected_countries_response);
    }
    #endregion


    #region GetCountryByCountryID
    //If you supply null country id it should return null
    [Fact]
    public async Task GetCountryByCountryID_NullCountryID_ToBeNull()
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
    public async Task GetCountryByCountryID_InvalidCountryID_ToBeNull()
    {
        //Arrange
        Guid countryID = Guid.NewGuid();

        _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>())).ReturnsAsync(null as Country);

        //Act
        CountryResponse? response = await _countriesService.GetCountryByCountryID(countryID);

        //Assert
        response?.Should().BeNull();
    }

    //If you supply true country id it should return proper country response
    [Fact]
    public async Task GetCountryByCountryID_ValidCountryID_ToBeSuccessful()
    {
        //Arrange
        Country country = _fixture.Build<Country>().With(temp => temp.Persons, null as List<Person>).Create();
        CountryResponse expected_country_response = country.ToCountryResponse();

        _countriesRepositoryMock.Setup(temp => temp.GetCountryByCountryID(It.IsAny<Guid>())).ReturnsAsync(country);
        //Act
        CountryResponse? response_from_get = await _countriesService.GetCountryByCountryID(country.CountryID);

        //Assert
        response_from_get.Should().Be(expected_country_response);
    }

    #endregion
}
