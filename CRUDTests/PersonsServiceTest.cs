﻿using System;
using Services;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using Xunit.Abstractions;


namespace CRUDTests
{
    public class PersonsServiceTest
    {
        //Private fields
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ITestOutputHelper _outputHelper;

        //Constructor
        public PersonsServiceTest(ITestOutputHelper outputHelper)
        {
            _personsService = new PersonsService(false);
            _countriesService = new CountriesService(false);
            _outputHelper = outputHelper;
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

        //Test services
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

            _outputHelper.WriteLine("Expected persons :");
            foreach(PersonResponse personResponse in persons_response_from_add)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }

            persons_response_from_get = _personsService.GetAllPersons();
            _outputHelper.WriteLine("Actual persons :");
            foreach (PersonResponse personResponse in persons_response_from_get)
            {
                _outputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            foreach (PersonResponse response in persons_response_from_add)
            {
                Assert.Contains(response, persons_response_from_get);
            }
        }
        #endregion


        #region GetPersonByPersonID
        // If you supply null person Id it should return null
        [Fact]
        public void GetPersonByPersonID_NullPersonID()
        {
            //Arrange
            Guid? personID = null;

            //Act
            PersonResponse? response = _personsService.GetPersonByPersonID(personID);

            //Assert
            Assert.Null(response);
        }

        //If you supply invalid person id then it should return null
        [Fact]
        public void GetPersonByPersonID_InvalidPersonID()
        {
            //Arrange
            Guid personID = Guid.NewGuid();

            //Act
            PersonResponse? response = _personsService.GetPersonByPersonID(personID);

            //Assert
            Assert.Null(response );
        }


        //If you supply valid person Id then it should return correct person response
        [Fact]
        public void GetPersonByPersonID_ValidPersonID()
        {
            //Arrange
            List<PersonAddRequest> persones_list = CreateSomePersons();
            PersonAddRequest person_add_request = persones_list[0];

            //Act
            PersonResponse person_response_from_add = _personsService.AddPerson(person_add_request);
            PersonResponse? person_response_from_get = _personsService.GetPersonByPersonID(person_response_from_add.PersonID);

            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
        }

        #endregion


        #region GetFiltredPersons
        //If the search text is empty and search by is "PersonName", it should return all persons
        [Fact]
        public void GetFiltredPerson_EmptySearchStraing()
        {
            //Arrange
            List<PersonAddRequest> persons_add_request_list = CreateSomePersons();
            List<PersonResponse> persons_response_from_add = new List<PersonResponse>();
            List<PersonResponse> persons_response_from_get = new List<PersonResponse>();

            //Act
            foreach(PersonAddRequest request in persons_add_request_list)
            {
                persons_response_from_add.Add(_personsService.AddPerson(request));
            }

            _outputHelper.WriteLine("Expected responses :");
            foreach(PersonResponse response in  persons_response_from_add)
            {
                _outputHelper.WriteLine(response.ToString());
            }

            persons_response_from_get = _personsService.GetFiltredPersons(nameof(PersonResponse.PersonName), "");
            _outputHelper.WriteLine("Actual responses :");
            foreach (PersonResponse response in persons_response_from_add)
            {
                _outputHelper.WriteLine(response.ToString());
            }

            //Assert
            foreach(PersonResponse personResponse in persons_response_from_add)
            {
                Assert.Contains(personResponse, persons_response_from_get);
            }
        }

        //First we will add few persons; and then we will search based on person name with some search string. It should return the matching persons
        [Fact]
        public void GetFiltredPerson_SearchByPersonName()
        {
            //Arrange
            List<PersonAddRequest> persons_add_request_list = CreateSomePersons();
            List<PersonResponse> persons_response_from_add = new List<PersonResponse>();
            List<PersonResponse> persons_response_from_get = new List<PersonResponse>();

            //Act
            foreach (PersonAddRequest request in persons_add_request_list)
            {
                persons_response_from_add.Add(_personsService.AddPerson(request));
            }

            _outputHelper.WriteLine("Added responses :");
            foreach (PersonResponse response in persons_response_from_add)
            {
                _outputHelper.WriteLine(response.ToString());
            }

            persons_response_from_get = _personsService.GetFiltredPersons(nameof(PersonResponse.PersonName), "ma");
            _outputHelper.WriteLine("Geted responses :");
            foreach (PersonResponse response in persons_response_from_get)
            {
                _outputHelper.WriteLine(response.ToString());
            }

            //Assert
            foreach (PersonResponse personResponse in persons_response_from_add)
            {
                if(personResponse.PersonName != null)
                {
                    if(personResponse.PersonName.Contains("ma", StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(personResponse, persons_response_from_get);
                    }
                }
            }
        }
        #endregion


        #region GetSortedPersons
        //When we sort based on PersonName in DESC, it should return persons list in descending on PersonName
        [Fact]
        public void GetSortedPersons_DESCOrder()
        {
            //Arrange
            List<PersonAddRequest> persons_add_request_list = CreateSomePersons();
            List<PersonResponse> persons_response_from_add = new List<PersonResponse>();
            List<PersonResponse> persons_response_from_get = new List<PersonResponse>();
            List<PersonResponse> persons_response_from_filter = new List<PersonResponse>();

            //Act
            foreach(PersonAddRequest person in persons_add_request_list)
            {
                persons_response_from_add.Add(_personsService.AddPerson(person));
            }

            List<PersonResponse> allPersons = _personsService.GetAllPersons();
            persons_response_from_filter = allPersons.OrderByDescending(temp => temp.PersonName).ToList();

            // print expected values
            _outputHelper.WriteLine("expected values: ");
            foreach(PersonResponse response in persons_response_from_filter)
            {
                _outputHelper.WriteLine(response.ToString());
            }

            persons_response_from_get = _personsService.GetSortedPersons(allPersons, nameof(PersonResponse.PersonName), SortOrderOptions.DESC);
            // print actual values
            _outputHelper.WriteLine("actual values: ");
            foreach (PersonResponse response in persons_response_from_get)
            {
                _outputHelper.WriteLine(response.ToString());
            }

            //Assert
            for(int i = 0; i < allPersons.Count; i++)
            {
                Assert.Equal(persons_response_from_filter[i], persons_response_from_get[i]);
            }
        }

        #endregion


        #region UpdatePerson
        //When we supply null as PersonUpdateRequest, it should throw ArgumentNullException
        [Fact]
        public void UpdatePerson_NullPersonUpdateRequest()
        {
            //Arrange
            PersonUpdateRequest? updateRequest = null;

            //Asert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personsService.UpdatePerson(updateRequest);
            });
        }

        //When we supply invalid person id, it should throw ArgumentException
        [Fact]
        public void UpdatePerson_InvalidPersonID()
        {
            //Arrange
            PersonUpdateRequest updateRequest = new PersonUpdateRequest()
            {
                PersonID = Guid.NewGuid(),
                PersonName = "Farzin",
                Email = "Example@email.com",
                Gender = GenderOptions.Male
            };

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                //Act
                _personsService.UpdatePerson(updateRequest);
            });
        }

        //When PersonName is null, it should throw ArgumentException
        [Fact]
        public void UpdatePerson_NullPersonName()
        {
            //Arrange
            List<PersonAddRequest> person_requests_list = CreateSomePersons();
            PersonAddRequest request = person_requests_list.First();

            //Act
            PersonResponse response_from_add = _personsService.AddPerson(request);
            PersonUpdateRequest personUpdateRequest = response_from_add.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;

            //Assert
            Assert.Throws<ArgumentException>(() =>
            {
                _personsService.UpdatePerson(personUpdateRequest);
            });
        }

        //First, add a new person and try to update the person name and email
        [Fact]
        public void UpdatePerson_ValidUpdateRequest()
        {
            //Arrange
            List<PersonAddRequest> person_requests_list = CreateSomePersons();
            PersonAddRequest request = person_requests_list.First();

            //Act
            PersonResponse response_from_add = _personsService.AddPerson(request);
            PersonUpdateRequest person_update_from_add = response_from_add.ToPersonUpdateRequest();
            person_update_from_add.PersonName = "Ali";
            person_update_from_add.Email = "sample@smaple.co";

            PersonResponse person_reponse_from_update = _personsService.UpdatePerson(person_update_from_add);
            PersonResponse? person_reponse_from_get = _personsService.GetPersonByPersonID(person_reponse_from_update.PersonID);

            //Assert
            Assert.Equal(person_reponse_from_update, person_reponse_from_get);
        }

        #endregion


        #region DeletePerson
        //If you supply null as PersonID, it should throw ArgumentNullException
        [Fact]
        public void DeletePerson_NullPersonID()
        {
            //Act
            Guid? personID = null;

            //Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                //Act
                _personsService.DeletePerson(personID);
            });
        }

        //If you supply an invalid PersonID, it should return false
        [Fact]
        public void DeletePerson_InvalidPersonID()
        {
            //Arrange
            Guid personID = Guid.NewGuid();

            //Act
            bool isDeleted = _personsService.DeletePerson(personID);

            //Assert
            Assert.False(isDeleted);
        }


        //If you supply an valid PersonID, it should return true
        [Fact]
        public void DeletePerson_ValidPersonID()
        {
            //Arrange
            List<PersonAddRequest> persons = CreateSomePersons();
            PersonAddRequest personAddRequest = persons.First();

            //Act
            PersonResponse personResponse = _personsService.AddPerson(personAddRequest);
            bool isDeleted = _personsService.DeletePerson(personResponse.PersonID);

            //Assert
            Assert.True(isDeleted);
        }

        #endregion
    }
}