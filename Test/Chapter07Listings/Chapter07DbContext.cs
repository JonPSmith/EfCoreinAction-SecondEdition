// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Test.Chapter06Listings;

namespace Test.Chapter07Listings
{
    public class Chapter07DbContext : DbContext
    {
        public Chapter07DbContext(
            DbContextOptions<Chapter07DbContext> options)
            : base(options)
        { }

        public DbSet<MyEntityClass> MyEntities { get; set; }

        public DbSet<Person> People { get; set; }

        public DbSet<IndexClass> IndexClasses { get; set; }

        protected override void OnModelCreating
            (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MyEntityClass>()
                .ToTable("MyTable");

            modelBuilder.Entity<MyEntityClass>()
                .Property(p => p.NormalProp)
                .HasColumnName( //#A
                    Database.IsSqlite() //#B
                        ? "SqliteDatabaseCol" //#C
                        : "GenericDatabaseCol"); //#C
        /*Database provider specific command example **************************
        #A In this case I am setting a column name, but the same would work for ToTable
        #B Each database provider has an extension called Is<DatabaseName> that returns true if the database is of that type
        #C Using the tests I pick a specific name for the column if its a Sqlite database, otherwise a generic name for any other database type
        * *******************************************************************/

            modelBuilder.Entity<MyEntityClass>()
                .Property<DateTime>("UpdatedOn"); //#A
            /*Shadow property******************************************************
            #A I use the Property<T> method to define the shadow property type
             * ********************************************************************/

            modelBuilder.Entity<MyEntityClass>()
                .Property(x => x.ReadOnlyIntMapped); //#A

            modelBuilder.Entity<Person>()
                .Property<DateTime>("DateOfBirth") //#A
                .HasField("_dateOfBirth"); //#B
        /*Backing fields ********************************************************* 
         #A I create a 'notional' property called DateOfBirth by which I can access this propery via EF Core. This also sets the column name in the database 
         #B Then I link it to a backing field _dateOfBirth
         * **************************************************************************/

            modelBuilder.Entity<IndexClass>()
                .HasIndex(p => p.IndexNonUnique);

            modelBuilder.Entity<IndexClass>()
                .HasIndex(p => p.IndexUnique)
                .IsUnique()
                .HasName("MyUniqueIndex");

            modelBuilder.Entity<MyEntityClass>()
                .Ignore(b => b.LocalString); //#A

            modelBuilder.Ignore<ExcludeClass>(); //#B
        }
    }
    /**Exclude section********************************************
    #A The Ignore method is used to exclude the property LocalString in the entity class, MyEntityClass, from being added to the database
    #B A different Ignore method can exclude a class such that if you have a property in an entity class of the Ignored type then that property is not added to the database
    * *********************************************/
}