using System;
using ServiceContracts.Enums;

using Entities;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Acts as a DTO for inserting a new person
    /// </summary>
    public class PersonAddRequest
    {
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReciveNewsLetters { get; set; }

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
                ReciveNewsLetters = this.ReciveNewsLetters
            };
        }
    }
}
