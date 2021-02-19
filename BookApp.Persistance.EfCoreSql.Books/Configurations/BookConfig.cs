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
            entity.HasIndex(x => x.ActualPrice);
            entity.HasIndex(x => x.ReviewsAverageVotes);
            entity.HasIndex(x => x.SoftDeleted);

            entity.HasOne(x => x.Details)
                .WithOne()
                .HasForeignKey<BookDetails>(x => x.BookDetailsId);

            //Had to manually configure a BookTag because EfCore.GenericServices can't (yet) handle index entities
            entity.HasMany(x => x.Tags)
                .WithMany(x => x.Books)
                .UsingEntity<BookTag>(
                    x => x.HasOne(x => x.Tag).WithMany().HasForeignKey(x => x.TagId),
                    x => x.HasOne(x => x.Book).WithMany().HasForeignKey(x => x.BookId));

        }
    }
}