// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

        public DbSet<ValueConversionExample> ConversionExamples { get; set; }

        public DbSet<BaseClass> BaseClasses { get; set; }
        public DbSet<DupClass> DupClasses { get; set; }

        protected override void OnModelCreating
            (ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ValueConversionExample>()
                .Property(e => e.StageViaFluent)
                .HasConversion<string>();

            modelBuilder.Entity<ValueConversionExample>()
                .Property(e => e.StageCanBeNull)
                .HasConversion<string>();

            var utcConverter = new ValueConverter<DateTime, DateTime>( //#A
                toDb => toDb,                                      //#B
                fromDb =>                                              //#C
                    DateTime.SpecifyKind(fromDb, DateTimeKind.Utc));   //#C

            modelBuilder.Entity<ValueConversionExample>()              //#D
                .Property(e => e.DateTimeUtcUtcOnReturn)               //#D
                .HasConversion(utcConverter);                          //#E
            /*********************************************************
            #A This creates a ValueConverter from DateTime to DateTime
            #B This saves the DateTime to the database in the normal way, i.e. no conversion
            #C On reading from the database we add the UTC setting to the DateTime
            #D This selects the property we want to configure
            #E And this adds the utcConverter to that property
             **********************************************/
            modelBuilder.Entity<ValueConversionExample>()
                .Property(e => e.DateTimeUtcSaveAsString)
                .HasConversion(new DateTimeToStringConverter());

            modelBuilder.Entity<BaseClass>()
                .ToTable("KeyTestTable");

            modelBuilder.Entity<DupClass>()
                .ToView("KeyTestTable");

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
                .Property<DateTime>("_dateOfBirth") //#A
                .HasColumnName("DateOfBirth"); //#B
            /*Backing fields ********************************************************* 
             #A This configures a field-only property with no linked public property 
             #B This sets the column name to "DateOfBirth"
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