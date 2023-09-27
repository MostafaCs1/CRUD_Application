using Services;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Xunit.Abstractions;
using Entities;
using AutoFixture;
using FluentAssertions;
using RepositoryContracts;
using Moq;
using System.Linq.Expressions;

namespace CRUDTests;

public class PersonsServiceTest
{
    //Private fields
    private readonly ITestOutputHelper _outputHelper;
    private readonly IFixture _fixture;

    private readonly IPersonsService _personsService;
    private readonly Mock<IPersonsRepository> _personsRepositoryMock;
    private readonly IPersonsRepository _personsRepository;

    //Constructor
    public PersonsServiceTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _fixture = new Fixture();

        //Mocking persons repository
        _personsRepositoryMock = new Mock<IPersonsRepository>();
        _personsRepository = _personsRepositoryMock.Object;

        _personsService = new PersonsService(_personsRepository);
    }

    //Test services
    #region AddPerson
    //If you supply null value it should return argument null exception
    [Fact]
    public async Task AddPerson_NullPerson_ToBeArgumentNullexception()
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
    public async Task AddPerson_NullPersonName_ToBeArgumentException()
    {
        //Arrange
        PersonAddRequest? request = _fixture.Build<PersonAddRequest>().With(temp => temp.PersonName, null as string).Create();

        //Act
        Func<Task> action = async () => await _personsService.AddPerson(request);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }


    //If you supply prop PersonAddRequest it should contain that person object in persons list
    [Fact]
    public async Task AddPerson_PropPerson_ToBeSuccessful()
    {
        //Arrange
        PersonAddRequest person_add_request = _fixture.Build<PersonAddRequest>().With(temp => temp.Email, "sample@email.com").Create();
        Person person = person_add_request.ToPerson();
        PersonResponse expected_person_response = person.ToPersonResponse();

        _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>())).ReturnsAsync(person);

        //Act
        PersonResponse person_response_from_add = await _personsService.AddPerson(person_add_request);
        expected_person_response.PersonID = person_response_from_add.PersonID;

        //Assert
        person_response_from_add.PersonID.Should().NotBe(Guid.Empty);
        person_response_from_add.Should().Be(expected_person_response);
    }

    #endregion 


    #region GetPersonByPersonID

    // If you supply null person Id it should return null
    [Fact]
    public async Task GetPersonByPersonID_NullPersonID_ToBeNull()
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
    public async Task GetPersonByPersonID_InvalidPersonID_ToBeNull()
    {
        //Arrange
        Guid personID = Guid.NewGuid();

        _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(null as Person);

        //Act
        PersonResponse? response = await _personsService.GetPersonByPersonID(personID);

        //Assert
        response.Should().BeNull();
    }


    //If you supply valid person Id then it should return correct person response
    [Fact]
    public async Task GetPersonByPersonID_ValidPersonID_ToBeSuccessful()
    {
        //Arrange
        Person person = _fixture.Build<Person>()
            .With(temp => temp.Email, "sample@email.com")
            .With(temp => temp.Country, null as Country).Create();

        PersonResponse expected_person_response = person.ToPersonResponse();

        _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);

        //Act
        PersonResponse? person_response_from_get = await _personsService.GetPersonByPersonID(expected_person_response.PersonID);

        //Assert
        person_response_from_get.Should().Be(expected_person_response);
    }

    #endregion


    #region GetAllPersons
    //persons list by default is empty
    [Fact]
    public async Task GetAllPersons_ToBeEmptyList()
    {
        //Arrange
        _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(new List<Person>());

        //Act
        List<PersonResponse> persons = await _personsService.GetAllPersons();

        //Assert
        persons.Should().BeEmpty();
    }

    //when we supply some person into list of persons it should return them
    [Fact]
    public async Task GetAllPersons_WithSomePerson_ToBeSuccessful()
    {
        //Arrange
        var persons = new List<Person>
        {
            _fixture.Build<Person>().With(temp => temp.Email, "sample_1@email.com")
            .With(temp => temp.Country, null as Country).Create(),
            _fixture.Build<Person>().With(temp => temp.Email, "sample_2@email.com")
            .With(temp => temp.Country, null as Country).Create(),
            _fixture.Build<Person>().With(temp => temp.Email, "sample_3@email.com")
            .With(temp => temp.Country, null as Country).Create()
        };

        List<PersonResponse> expected_persons_response = persons.Select(person => person.ToPersonResponse()).ToList();

        //mocking GetAllPersons 
        _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

        //Act
        _outputHelper.WriteLine("Expected persons :");
        foreach (PersonResponse personResponse in expected_persons_response)
        {
            _outputHelper.WriteLine(personResponse.ToString());
        }

        List<PersonResponse> persons_response_from_get = await _personsService.GetAllPersons();
        _outputHelper.WriteLine("Actual persons :");
        foreach (PersonResponse personResponse in persons_response_from_get)
        {
            _outputHelper.WriteLine(personResponse.ToString());
        }

        //Assert
        persons_response_from_get.Should().BeEquivalentTo(expected_persons_response);
    }
    #endregion


    #region GetFiltredPersons
    //If the search text is empty and search by is "PersonName", it should return all persons
    [Fact]
    public async Task GetFiltredPerson_EmptySearchStreaing_ToBeSuccessful()
    {
        //Arrange
        var persons = new List<Person>
        {
            _fixture.Build<Person>().With(temp => temp.Email, "sample_1@email.com")
            .With(temp => temp.Country, null as Country).Create(),
            _fixture.Build<Person>().With(temp => temp.Email, "sample_2@email.com")
            .With(temp => temp.Country, null as Country).Create(),
            _fixture.Build<Person>().With(temp => temp.Email, "sample_3@email.com")
            .With(temp => temp.Country, null as Country).Create()
        };

        List<PersonResponse> expected_persons_response = persons.Select(person => person.ToPersonResponse()).ToList();

        _personsRepositoryMock.Setup(temp => temp.GetFiltredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(persons);
        _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

        //Act
        _outputHelper.WriteLine("Expected responses :");
        foreach (PersonResponse response in expected_persons_response)
        {
            _outputHelper.WriteLine(response.ToString());
        }

        List<PersonResponse> persons_response_from_get = await _personsService.GetFiltredPersons(nameof(PersonResponse.PersonName), "");
        _outputHelper.WriteLine("Actual responses :");
        foreach (PersonResponse response in persons_response_from_get)
        {
            _outputHelper.WriteLine(response.ToString());
        }

        //Assert
        persons_response_from_get.Should().BeEquivalentTo(expected_persons_response);
    }

    //First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
    [Fact]
    public async Task GetFiltredPerson_SearchByPersonName_ToBeSuccessful()
    {
        //Arrange
        var persons = new List<Person>
        {
            _fixture.Build<Person>()
            .With(temp => temp.Email, "sample_1@email.com")
            .With(temp => temp.PersonName, "Mahdi")
            .With(temp => temp.Country, null as Country).Create(),
            _fixture.Build<Person>()
            .With(temp => temp.Email, "sample_2@email.com")
            .With(temp => temp.PersonName, "Rahman")
            .With(temp => temp.Country, null as Country).Create(),
            _fixture.Build<Person>()
            .With(temp => temp.Email, "sample_3@email.com")
            .With(temp => temp.PersonName, "Ali")
            .With(temp => temp.Country, null as Country).Create()
        };


        List<Person> expected_persons = persons
            .Where(person => !string.IsNullOrEmpty(person.PersonName) && person.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase)).ToList();

        List<PersonResponse> expected_persons_response = expected_persons.Select(person => person.ToPersonResponse()).ToList();

        _personsRepositoryMock.Setup(temp => temp.GetFiltredPersons(It.IsAny<Expression<Func<Person, bool>>>())).ReturnsAsync(expected_persons);
        _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

        //Act
        _outputHelper.WriteLine("Added responses :");
        foreach (PersonResponse response in expected_persons_response)
        {
            _outputHelper.WriteLine(response.ToString());
        }

        List<PersonResponse> persons_response_from_get = await _personsService.GetFiltredPersons(nameof(PersonResponse.PersonName), "ma");
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
    public void GetSortedPersons_DESCOrder_ToBeSuccessful()
    {
        //Arrange
        var persons = new List<Person>
        {
            _fixture.Build<Person>()
            .With(temp => temp.Email, "sample_1@email.com")
            .With(temp => temp.PersonName, "Mahdi")
            .With(temp => temp.Country, null as Country).Create(),
            _fixture.Build<Person>()
            .With(temp => temp.Email, "sample_2@email.com")
            .With(temp => temp.PersonName, "Rahman")
            .With(temp => temp.Country, null as Country).Create(),
            _fixture.Build<Person>()
            .With(temp => temp.Email, "sample_3@email.com")
            .With(temp => temp.PersonName, "Ali")
            .With(temp => temp.Country, null as Country).Create()
        };

        List<PersonResponse> allPersons = persons.Select(person => person.ToPersonResponse()).ToList();

        //Act
        List<PersonResponse> persons_response_from_get = _personsService.GetSortedPersons(allPersons, nameof(PersonResponse.PersonName), SortOrderOptions.DESC);
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
    public async Task UpdatePerson_NullPersonUpdateRequest_ToBeArgumentNullException()
    {
        //Arrange
        PersonUpdateRequest? updateRequest = null;

        //Act
        Func<Task> action = async () => await _personsService.UpdatePerson(updateRequest);

        //Asert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    //When we supply invalid person id, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_InvalidPersonID_ToBeArgumentException()
    {
        //Arrange
        PersonUpdateRequest updateRequest = _fixture.Build<PersonUpdateRequest>()
            .With(temp => temp.Email, "sample@email.com").Create();

        //Act
        Func<Task> action = async () => await _personsService.UpdatePerson(updateRequest);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    //When PersonName is null, it should throw ArgumentException
    [Fact]
    public async Task UpdatePerson_PersonNameIsNull_ToBeArgumentException()
    {
        //Arrange
        Person person = _fixture.Build<Person>()
            .With(temp => temp.PersonName, null as string)
            .With(temp => temp.Email, "sample@email.com")
            .With(temp => temp.Gender, "Male")
            .With(temp => temp.Country, null as Country).Create();

        PersonResponse personResponse = person.ToPersonResponse();
        PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

        //Act
        Func<Task> action = async () => await _personsService.UpdatePerson(personUpdateRequest);

        //Assert
        await action.Should().ThrowAsync<ArgumentException>();
    }

    //First, add a new person and try to update the person name and email
    [Fact]
    public async void UpdatePerson_ValidUpdateRequest_ToBeSuccessful()
    {
        //Arrange
        Person person = _fixture.Build<Person>()
            .With(temp => temp.PersonName, "Mahdi")
            .With(temp => temp.Email, "sample@email.com")
            .With(temp => temp.Country, null as Country)
            .With(temp => temp.Gender, "Male").Create();

        PersonResponse expected_person_response = person.ToPersonResponse();
        PersonUpdateRequest person_update_request = expected_person_response.ToPersonUpdateRequest();

        _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);
        _personsRepositoryMock.Setup(temp => temp.UpdatePerson(It.IsAny<Person>())).ReturnsAsync(person);

        //Act
        PersonResponse person_reponse_from_update = await _personsService.UpdatePerson(person_update_request);

        //Assert
        person_reponse_from_update.Should().Be(expected_person_response);
    }

    #endregion


    #region DeletePerson
    //If you supply null as PersonID, it should throw ArgumentNullException
    [Fact]
    public async Task DeletePerson_NullPersonID_ToBeArgumentNullException()
    {
        //Act
        Guid? personID = null;

        //Act
        Func<Task> action = async () => await _personsService.DeletePerson(personID);

        //Assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    //If you supply an invalid PersonID, it should return false
    [Fact]
    public async Task DeletePerson_InvalidPersonID_ToBeFalse()
    {
        //Arrange
        Guid personID = Guid.NewGuid();

        _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(null as Person);

        //Act
        bool isDeleted = await _personsService.DeletePerson(personID);

        //Assert
        isDeleted.Should().BeFalse();
    }


    //If you supply an valid PersonID, it should return true
    [Fact]
    public async Task DeletePerson_ValidPersonID_ToBeTrue()
    {
        //Arrange
        Person person = _fixture.Build<Person>()
            .With(temp => temp.Email, "sample@email.com")
            .With(temp => temp.Country, null as Country).Create();

        _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(person);
        _personsRepositoryMock.Setup(temp => temp.DeletePersonByPersonID(It.IsAny<Guid>())).ReturnsAsync(true);

        //Act
        bool isDeleted = await _personsService.DeletePerson(person.PersonID);

        //Assert
        isDeleted.Should().BeTrue();
    }

    #endregion
}