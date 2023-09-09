using System;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class PersonDbContext : DbContext
    {
        //constractor
        public PersonDbContext(DbContextOptions options) : base(options) { }

        //define tables
        public DbSet<Person> Persons { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //map models into tables
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //seed countries
            string countriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);
            foreach (Country country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            //seed persons
            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);
            foreach (Person prson in persons)
            {
                modelBuilder.Entity<Person>().HasData(prson);
            }

            //fluent API
            //modelBuilder.Entity<Person>().HasIndex(person => person.TIN).IsUnique();

            //change TIN column name and type
            modelBuilder.Entity<Person>().Property(temp => temp.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABC12345");

            //add Constraint into TIN column
            modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8");

            // table relation
            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasOne<Country>(p => p.Country)
                .WithMany(c => c.Persons)
                .HasForeignKey("countryID");
            });
        }

    }
}
