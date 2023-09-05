using Entities;
using ServiceContracts.Enums;
using System;
using System.ComponentModel.DataAnnotations;


namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents the DTO class that contains the person details to update
    /// </summary>
    public class PersonUpdateRequest
    {
        [Required(ErrorMessage = "Person Id can't be blank.")]
        public Guid PersonID { get; set; }
        
        [Required(ErrorMessage = "Person name can't be empty.")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email field can't be empty or null.")]
        [EmailAddress(ErrorMessage = "Email address isn't valid.")]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Person gender can't be empty choose one.")]
        public GenderOptions? Gender { get; set; }
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
