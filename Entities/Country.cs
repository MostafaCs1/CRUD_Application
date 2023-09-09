using System.ComponentModel.DataAnnotations;

namespace Entities
{
    /// <summary>
    /// Country domain model class
    /// </summary>
    public class Country
    {
        [Key]
        public Guid? CountryID { get; set; }

        [StringLength(40)] // nvarchar(50)
        public string? CountryName { get; set; }

        //collection of persons for corresponding country ID
        public virtual ICollection<Person>? Persons { get; set; }
    }
}