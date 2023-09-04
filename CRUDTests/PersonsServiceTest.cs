using System;
using Services;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;


namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //Private fields
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;

        //Constructor
        public PersonsServiceTest()
        {
            _personsService = new PersonsService();
            _countriesService = new CountriesService();
        }

        //Methods
        public List<PersonAddRequest> CreateSomePersons()
        {
            //create some country
            List<CountryAddRequest> requests = new List<CountryAddRequest>{
                new CountryAddRequest { CountryName = "Iran" },
                new CountryAddRequest { CountryName = "India"},
                new CountryAddRequest { CountryName = "USA"}
            };
            List<CountryResponse> responses = new List<CountryResponse>();
            foreach (CountryAddRequest request in requests)
            {
                responses.Add(_countriesService.AddCountry(request));
            }

            //create somse person
            List<PersonAddRequest> personAddRequest_list = new List<PersonAddRequest>(){
            new PersonAddRequest()
            {
                PersonName = "Mahdi",
                Email = "example@email.com",
                CountryID = responses[0].CountryID,
                ReceiveNewsLetters = true,
                Gender = GenderOptions.Female,
                Address = "address",
                DateOfBirth = DateTime.Parse("2000-05-06") },
            new PersonAddRequest()
            {
                PersonName = "Farzin",
                Email = "farzin@email.com",
                CountryID = responses[1].CountryID,
                ReceiveNewsLetters = true,
                Gender = GenderOptions.Male,
                Address = "address street 52",
                DateOfBirth = DateTime.Parse("2005-05-06")
            },
            new PersonAddRequest()
            {
                PersonName = "Rahman",
                Email = "farzin@email.com",
                CountryID = responses[2].CountryID,
                ReceiveNewsLetters = true,
                Gender = GenderOptions.Male,
                Address = "address street 52",
                DateOfBirth = DateTime.Parse("2005-05-06")
            }
            };

            return personAddRequest_list;

        }

        #region AddPerson
        //If you supply null value it should return argument null exception
        [Fact]
        public void AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? request = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personsService.AddPerson(request);
            });
        }

        //If you supply null Person name then it should return argument exception
        [Fact]
        public void AddPerson_NullPersonName()
        {
            //Arrange
            PersonAddRequest? request = new PersonAddRequest { PersonName = null };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personsService.AddPerson(request);
            });
        }


        //If you supply prop PersonAddRequest it should contain that person object in persons list
        [Fact]
        public void AddPerson_PropPerson()
        {
            //Arrange
            List<PersonAddRequest> personAddRequest_list = CreateSomePersons();
            PersonAddRequest request = personAddRequest_list.First();

            //Act
            PersonResponse response_from_add = _personsService.AddPerson(request);
            List<PersonResponse> get_persons_list = _personsService.GetAllPersons();

            //Assert
            Assert.True(response_from_add.PersonID != Guid.Empty);
            Assert.Contains(response_from_add, get_persons_list);
        }

        #endregion


        #region GetAllPersons
        //persons list by default is empty
        [Fact]
        public void GetAllPersons_EmptyList()
        {
            //Act
            List<PersonResponse> persons = _personsService.GetAllPersons();

            //Assert
            Assert.Empty(persons);
        }

        //when we supply some person into list of persons it should return them
        [Fact]
        public void GetAllPersons_AddSomePerson()
        {
            //Arrange
            List<PersonAddRequest> persons_add_request = CreateSomePersons();
            List<PersonResponse> persons_response_from_add = new List<PersonResponse>();
            List<PersonResponse> persons_response_from_get = new List<PersonResponse>();

            //Act
            foreach(PersonAddRequest request in persons_add_request)
            {
                PersonResponse response = _personsService.AddPerson(request);
                persons_response_from_add.Add(response);
            }
            persons_response_from_get = _personsService.GetAllPersons();

            //Assert
            foreach(PersonResponse response in persons_response_from_add)
            {
                Assert.Contains(response, persons_response_from_get);
            }
        }
        #endregion
    }
}
