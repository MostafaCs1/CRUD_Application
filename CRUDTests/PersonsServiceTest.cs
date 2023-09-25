using Services;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Entities;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;

namespace CRUDTests;

public class PersonsServiceTest
{
    //Private fields
    private readonly ITestOutputHelper _outputHelper;
    private readonly IFixture _fixture;

    private readonly IPersonsService _personsService;

    //Constructor
    public PersonsServiceTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _fixture = new Fixture();

        //Initial tables
        var personsInitialData = new List<Person>() { };

        //Get instance of dbContext builder option
        DbContextOptions builderOption = new DbContextOptionsBuilder<ApplicationDbContext>().Options;

        //mocking dbContext
        var dbContextMock = new DbContextMock<ApplicationDbContext>(builderOption);

        //add mock tables into mock dbContext
        dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

        _personsService = new PersonsService(dbContextMock.Object);
    }

    //Test services
    #region AddPerson
    //If you supply null value it should return argument null exception
    [Fact]
    public async Task AddPerson_NullPerson()
    {
        //Arrange
        PersonAddRequest? request = null;

        //Act
        Func<Task> action = async () => await _personsService.AddPerson(request);

        //Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    //If you supply null Person name then it should return argument exception
    [Fact]
    public async Task AddPerson_NullPersonName()
    {
        //Arrange
        PersonAddRequest? request = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

        //Act
        Func<Task> action = async () => await _personsService.AddPerson(request);
        Task newVal = action.Invoke();

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }


    //If you supply prop PersonAddRequest it should contain that person object in persons list
    [Fact]
    public async Task AddPerson_PropPerson()
    {
        //Arrange
        PersonAddRequest request = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample@email.com").Create();

        //Act
        PersonResponse response_from_add = await _personsService.AddPerson(request);
        List<PersonResponse> get_persons_list = await _personsService.GetAllPersons();

        //Assert
        response_from_add.PersonID.Should().NotBe(Guid.Empty);
        get_persons_list.Should().Contain(response_from_add);
    }

    #endregion 


    #region GetPersonByPersonID

    // If you supply null person Id it should return null
    [Fact]
    public async Task GetPersonByPersonID_NullPersonID()
    {
        //Arrange
        Guid? personID = null;

        //Act
        PersonResponse? response = await _personsService.GetPersonByPersonID(personID);

        //Assert
        response.Should().BeNull();
    }

    //If you supply invalid person id then it should return null
    [Fact]
    public async Task GetPersonByPersonID_InvalidPersonID()
    {
        //Arrange
        Guid personID = Guid.NewGuid();

        //Act
        PersonResponse? response = await _personsService.GetPersonByPersonID(personID);

        //Assert
        response.Should().BeNull();
    }


    //If you supply valid person Id then it should return correct person response
    [Fact]
    public async Task GetPersonByPersonID_ValidPersonID()
    {
        //Arrange
        PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample@email.com").Create();

        //Act
        PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);
        PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

        //Assert
        person_response_from_get.Should().Be(person_response_from_add);
    }

    #endregion


    #region GetAllPersons
    //persons list by default is empty
    [Fact]
    public async Task GetAllPersons_EmptyList()
    {
        //Act
        List<PersonResponse> persons = await _personsService.GetAllPersons();

        //Assert
        persons.Should().BeEmpty();
    }

    //when we supply some person into list of persons it should return them
    [Fact]
    public async Task GetAllPersons_AddSomePerson()
    {
        //Arrange
        var persons_add_request = new List<PersonAddRequest>
        {
            _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample_1@email.com").Create(),
            _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample_2@email.com").Create(),
            _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample_3@email.com").Create()
        };

        var persons_response_from_add = new List<PersonResponse>();
        var persons_response_from_get = new List<PersonResponse>();

        //Act
        foreach (PersonAddRequest request in persons_add_request)
        {
            PersonResponse response = await _personsService.AddPerson(request);
            persons_response_from_add.Add(response);
        }

        _outputHelper.WriteLine("Expected persons :");
        foreach (PersonResponse personResponse in persons_response_from_add)
        {
            _outputHelper.WriteLine(personResponse.ToString());
        }

        persons_response_from_get = await _personsService.GetAllPersons();
        _outputHelper.WriteLine("Actual persons :");
        foreach (PersonResponse personResponse in persons_response_from_get)
        {
            _outputHelper.WriteLine(personResponse.ToString());
        }

        //Assert
        persons_response_from_get.Should().BeEquivalentTo(persons_response_from_add);
    }
    #endregion


    #region GetFiltredPersons
    //If the search text is empty and search by is "PersonName", it should return all persons
    [Fact]
    public async Task GetFiltredPerson_EmptySearchStraing()
    {
        //Arrange
        var persons_add_request_list = new List<PersonAddRequest>
        {
            _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample_1@email.com").Create(),
            _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample_2@email.com").Create(),
            _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample_3@email.com").Create()
        };
        var persons_response_from_add = new List<PersonResponse>();
        var persons_response_from_get = new List<PersonResponse>();

        //Act
        foreach (PersonAddRequest request in persons_add_request_list)
        {
            persons_response_from_add.Add(await _personsService.AddPerson(request));
        }

        _outputHelper.WriteLine("Expected responses :");
        foreach (PersonResponse response in persons_response_from_add)
        {
            _outputHelper.WriteLine(response.ToString());
        }

        persons_response_from_get = await _personsService.GetFiltredPersons(nameof(PersonResponse.PersonName), "");
        _outputHelper.WriteLine("Actual responses :");
        foreach (PersonResponse response in persons_response_from_add)
        {
            _outputHelper.WriteLine(response.ToString());
        }

        //Assert
        persons_response_from_get.Should().BeEquivalentTo(persons_response_from_add);
    }

    //First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
    [Fact]
    public async Task GetFiltredPerson_SearchByPersonName()
    {
        //Arrange
        var persons_add_request_list = new List<PersonAddRequest>
        {
            _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "sample_1@email.com").With(temp => temp.PersonName, "Mahdi").Create(),
            _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "sample_2@email.com").With(temp => temp.PersonName, "Rahman").Create(),
            _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "sample_3@email.com").With(temp => temp.PersonName, "Ali").Create()
        };

        var persons_response_from_add = new List<PersonResponse>();
        var persons_response_from_get = new List<PersonResponse>();

        //Act
        foreach (PersonAddRequest request in persons_add_request_list)
        {
            persons_response_from_add.Add(await _personsService.AddPerson(request));
        }

        _outputHelper.WriteLine("Added responses :");
        foreach (PersonResponse response in persons_response_from_add)
        {
            _outputHelper.WriteLine(response.ToString());
        }

        persons_response_from_get = await _personsService.GetFiltredPersons(nameof(PersonResponse.PersonName), "ma");
        _outputHelper.WriteLine("Geted responses :");
        foreach (PersonResponse response in persons_response_from_get)
        {
            _outputHelper.WriteLine(response.ToString());
        }

        //Assert
        persons_response_from_get.Should().OnlyContain(
            person => !string.IsNullOrEmpty(person.PersonName) && person.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase)
            );
    }
    #endregion


    #region GetSortedPersons
    //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
    [Fact]
    public async Task GetSortedPersons_DESCOrder()
    {
        //Arrange
        var persons_add_request_list = new List<PersonAddRequest>
        {
            _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "sample_1@email.com").With(temp => temp.PersonName, "Mahdi").Create(),
            _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "sample_2@email.com").With(temp => temp.PersonName, "Rahman").Create(),
            _fixture.Build<PersonAddRequest>()
            .With(temp => temp.Email, "sample_3@email.com").With(temp => temp.PersonName, "Ali").Create()
        };

        var persons_response_from_add = new List<PersonResponse>();
        var persons_response_from_get = new List<PersonResponse>();
        var persons_response_from_filter = new List<PersonResponse>();

        //Act
        foreach (PersonAddRequest person in persons_add_request_list)
        {
            persons_response_from_add.Add(await _personsService.AddPerson(person));
        }

        List<PersonResponse> allPersons = await _personsService.GetAllPersons();

        persons_response_from_get = _personsService.GetSortedPersons(allPersons, nameof(PersonResponse.PersonName), SortOrderOptions.DESC);
        // print actual values
        _outputHelper.WriteLine("actual values: ");
        foreach (PersonResponse response in persons_response_from_get)
        {
            _outputHelper.WriteLine(response.ToString());
        }

        //Assert
        persons_response_from_get.Should().BeInDescendingOrder(temp => temp.PersonName);
    }

    #endregion


    #region UpdatePerson
    //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
    [Fact]
    public async Task UpdatePerson_NullPersonUpdateRequest()
    {
        //Arrange
        PersonUpdateRequest? updateRequest = null;

        //Act
        Func<Task> action = async() => await _personsService.UpdatePerson(updateRequest);

        //Asert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    //When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonID()
    {
        //Arrange
        PersonUpdateRequest updateRequest = _fixture.Build<PersonUpdateRequest>()
            .With(temp => temp.Email, "sample@email.com").Create();

        //Act
        Func<Task> action = async() => await _personsService.UpdatePerson(updateRequest);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    //When PersonName is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_PersonNameIsNull()
    {
        //Arrange
        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample@email.com").Create();

        //Act
        PersonResponse response_from_add = await _personsService.AddPerson(personAddRequest);
        PersonUpdateRequest personUpdateRequest = response_from_add.ToPersonUpdateRequest();
        personUpdateRequest.PersonName = null;

        Func<Task> action = async() => await _personsService.UpdatePerson(personUpdateRequest);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    //First, add a new person and try to update the person name and email
    [Fact]
    public async void UpdatePerson_ValidUpdateRequest()
    {
        //Arrange
        PersonAddRequest request = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample_2@email.com").Create();

        //Act
        PersonResponse response_from_add = await _personsService.AddPerson(request);
        PersonUpdateRequest person_update_from_add = response_from_add.ToPersonUpdateRequest();
        person_update_from_add.PersonName = "Ali";
        person_update_from_add.Email = "sample@smaple.co";

        PersonResponse person_reponse_from_update = await _personsService.UpdatePerson(person_update_from_add);
        PersonResponse? person_reponse_from_get = await _personsService.GetPersonByPersonID(person_reponse_from_update.PersonID);

        //Assert
        person_reponse_from_get.Should().Be(person_reponse_from_update);
    }

    #endregion


    #region DeletePerson
    //If you supply null as PersonID, it should throw ArgumentNullException
    [Fact]
    public async Task DeletePerson_NullPersonID()
    {
        //Act
        Guid? personID = null;

        //Act
        Func<Task> action = async() => await _personsService.DeletePerson(personID);

        //Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    //If you supply an invalid PersonID, it should return false
    [Fact]
    public async Task DeletePerson_InvalidPersonID()
    {
        //Arrange
        Guid personID = Guid.NewGuid();

        //Act
        bool isDeleted = await _personsService.DeletePerson(personID);

        //Assert
        isDeleted.Should().BeFalse();
    }


    //If you supply an valid PersonID, it should return true
    [Fact]
    public async Task DeletePerson_ValidPersonID()
    {
        //Arrange
        PersonAddRequest personAddRequest = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample@email.com").Create();

        //Act
        PersonResponse personResponse = await _personsService.AddPerson(personAddRequest);
        bool isDeleted = await _personsService.DeletePerson(personResponse.PersonID);

        //Assert
        isDeleted.Should().BeTrue();
    }

    #endregion
}