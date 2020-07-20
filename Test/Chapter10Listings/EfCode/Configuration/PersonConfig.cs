// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

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
            entity.Property<DateTime>("_dateOfBirth")
                .HasColumnName("DateOfBirth");

            entity.Property(p => p.YearOfBirth) //#B
                .HasComputedColumnSql(                  //#B
                    "DatePart(yyyy, [DateOfBirth])");   //#B
        }
        /**************************************************************
         * 
         * ***********************************************************/
    }
}
