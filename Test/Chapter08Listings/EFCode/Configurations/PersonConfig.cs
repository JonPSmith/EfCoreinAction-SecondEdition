// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter08Listings.EfClasses;

namespace Test.Chapter08Listings.EFCode.Configurations
{
    public class PersonConfig : IEntityTypeConfiguration<Person>
    {
        public void Configure
            (EntityTypeBuilder<Person> entity)
        {
            entity
                .HasOne(p => p.ContactInfo)
                .WithOne()
                .HasForeignKey<ContactInfo>(p => p.EmailAddress)
                .HasPrincipalKey<Person>(c => c.Name);
        }
    }
}