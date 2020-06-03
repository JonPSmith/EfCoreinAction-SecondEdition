// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter08Listings.SplitOwnClasses;

namespace Test.Chapter08Listings.EFCode.Configurations
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure
            (EntityTypeBuilder<User> entity)
        {
            entity
                .OwnsOne(e => e.HomeAddress)
                .ToTable("Addresses");
        }
    }
}