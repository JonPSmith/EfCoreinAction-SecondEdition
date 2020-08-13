// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookApp.Persistence.NormalSql.Book.Configurations
{
    internal class BookConfig : IEntityTypeConfiguration<Domain.Book.Book>
    {
        public void Configure(EntityTypeBuilder<Domain.Book.Book> entity)
        {
            entity.Property(p => p.PublishedOn)
                .HasColumnType("date");

            entity.HasIndex(x => x.PublishedOn);

        }
    }
}