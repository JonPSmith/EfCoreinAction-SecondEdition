// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter10Listings.EfClasses;

namespace Test.Chapter10Listings.EfCode.Configuration
{
    public class PersonConfig : IEntityTypeConfiguration<Person>
    {
        public void Configure
            (EntityTypeBuilder<Person> entity)
        {
            entity.Property<DateTime>("_dateOfBirth")   //#A
                .HasColumnName("DateOfBirth");          //#A

            entity.Property(p => p.YearOfBirth)         //#B
                .HasComputedColumnSql(                  //#B
                    "DatePart(yyyy, [DateOfBirth])");   //#B

            entity.Property(p => p.FullName)            //#C
                .HasComputedColumnSql(                  //#C
                    "[FirstName] + ' ' + [LastName]",   //#C
                    stored: true);                      //#C

            entity.HasIndex(x => x.FullName);      //#D 
            //You can add a index to computed column if it is deterministic
            //entity.HasIndex(x => x.YearOfBirth);
        }
    }
    /*****************************************************************
    #A Configures the backing field, with the column name DateOfBirth
    #B Configures the property as a computed column and provides the SQL code that the database server will run 
    #C Configures the property as a persistent computed column and provides the SQL code that the database server will run
    #D Adds an index to the FullName column because you want to filter/sort on that column
     ****************************************************/
}
