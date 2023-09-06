using System;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Acts as a DTO for inserting a new person
    /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Person name can't be blanck.")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email field can't be blank.")]
        [EmailAddress(ErrorMessage = "Email address isn't valid.")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date of birth field can't be empty.")]
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }

        [Required(ErrorMessage = "Please selecet a country.")]
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        /// <summary>
        /// Convert current PersonAddRequest type into person object
        /// </summary>
        /// <returns>Person object type</returns>
        public Person ToPerson()
        {
            return new Person
            {
                PersonName = this.PersonName,
                Email = this.Email,
                DateOfBirth = this.DateOfBirth,
                Gender = this.Gender.ToString(),
                CountryID = this.CountryID,
                Address = this.Address,
                ReceiveNewsLetters = this.ReceiveNewsLetters
            };
        }
    }
}
