﻿using System.ComponentModel.DataAnnotations;


namespace Entities
{
    /// <summary>
    /// Person domain model class
    /// </summary>
    public class Person
    {
        [Key] // primiray key
        public Guid PersonID { get; set; }

        [StringLength(40)] // nvarchar(40)
        public string? PersonName { get; set; }

        [StringLength(40)]
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [StringLength(6)]
        public string? Gender { get; set; }
        public Guid? CountryID { get; set; }

        [StringLength(400)]
        public string? Address { get; set; }

        //bit type
        public bool ReceiveNewsLetters { get; set; }

        public string? TIN { get; set; }

    }
}
