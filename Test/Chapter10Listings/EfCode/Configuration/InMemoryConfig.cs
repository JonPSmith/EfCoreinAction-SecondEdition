// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter10Listings.EfClasses;

namespace Test.Chapter10Listings.EfCode.Configuration
{
    public class InMemoryConfig : IEntityTypeConfiguration<InMemoryTest>
    {
        public void Configure
            (EntityTypeBuilder<InMemoryTest> entity)
        {
            entity.IsMemoryOptimized();
        }
    }
}
