using Entities;
using System;

namespace ServiceContracts.DTO
{
    /// <summary>
    /// Represents DTO class that is used as return type of most methods of Persons Service
    /// </summary>
    public class PersonResponse
    {
        public Guid PersonID { get; set; }
        public string? PersonName { get; set; }
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public double? Age { get; set; }
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReciveNewsLetters { get; set; }

        /// <summary>
        /// Compare current object with param object
        /// </summary>
        /// <param name="obj">compare object</param>
        /// <returns>If they are equal return true, otherwie return false</returns>
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if(obj.GetType() != typeof(PersonResponse))
                return false;

            PersonResponse response = (PersonResponse)obj;
            bool isEqual = this.PersonID == response.PersonID && this.PersonName == response.PersonName && this.Email == response.Email && this.DateOfBirth == response.DateOfBirth && this.Gender == response.Gender && this.CountryID == response.CountryID && this.Address == response.Address && this.ReciveNewsLetters == response.ReciveNewsLetters;

            return isEqual;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public static class PersonExtention
    {
        /// <summary>
        /// Change Person object to PersonResponse type
        /// </summary>
        /// <param name="person">The Person object to convert</param>
        /// <returns>Returns the converted PersonResponse object</returns>
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Address = person.Address,
                ReciveNewsLetters = person.ReciveNewsLetters,
                Age = (person.DateOfBirth != null) ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null
            };
        }
    }


}
