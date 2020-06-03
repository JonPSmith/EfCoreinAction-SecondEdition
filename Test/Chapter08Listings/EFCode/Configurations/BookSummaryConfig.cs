// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter08Listings.SplitOwnClasses;

namespace Test.Chapter08Listings.EFCode.Configurations
{
    public class BookSummaryConfig : IEntityTypeConfiguration<BookSummary>
    {
        public void Configure
            (EntityTypeBuilder<BookSummary> entity)
        {
            entity
                .HasOne(e => e.Details).WithOne()
                .HasForeignKey<BookDetail>(e => e.BookDetailId);
            entity.ToTable("Books");
        }
    }
}