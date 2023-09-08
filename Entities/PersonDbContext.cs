using System;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class PersonDbContext : DbContext
    {
        //define tables
        public DbSet<Person> Persons { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //rename tebles
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");
        }

    }
}
