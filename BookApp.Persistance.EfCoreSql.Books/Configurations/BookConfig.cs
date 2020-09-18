// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BookApp.Domain.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookApp.Persistence.EfCoreSql.Books.Configurations
{
    internal class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> entity)
        {
            entity.HasIndex(x => x.PublishedOn);
            entity.HasIndex(x => x.ReviewsAverageVotes);

            entity.HasOne(x => x.Details)
                .WithOne()
                .HasForeignKey<BookDetails>(x => x.BookDetailsId);

            //Had to manually configure a BookTag because GenericServices can't (yet) handle index entities
            entity.HasMany(e => e.Tags)
                .WithMany(e => e.Books)
                .UsingEntity<BookTag>(
                    b => b.HasOne(e => e.Tag).WithMany().HasForeignKey(e => e.TagId),
                    b => b.HasOne(e => e.Book).WithMany().HasForeignKey(e => e.BookId));

        }
    }
}