// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.EfCode.Configurations
{
    internal class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure
            (EntityTypeBuilder<Book> entity)
        {
            entity.Property(p => p.PublishedOn)//#A
                .HasColumnType("date");        

            entity.Property(p => p.Price) //#B
                .HasColumnType("decimal(9,2)");

            entity.Property(x => x.ImageUrl) //#C
                .IsUnicode(false);

            entity.HasIndex(x => x.PublishedOn); //#D

            //Model-level query filter

            entity
                .HasQueryFilter(p => !p.SoftDeleted); //#E

            //----------------------------
            //relationships

            entity.HasOne(p => p.Promotion) //#A
                .WithOne() //#A
                .HasForeignKey<PriceOffer>(p => p.BookId); //#A

            entity.HasMany(p => p.Reviews)     //#B
                .WithOne()                     //#B
                .HasForeignKey(p => p.BookId); //#B
        }
    }
    /*Type/Size setting**********************************************
    #A The convention-based mapping for .NET DateTime is SQL datetime2. This command changes the SQL column type to date, which only holds the date, not time
    #B I set a smaller precision and scale of (9,2) for the price instead of the default (18,2)
    #C The convention-based mapping for .NET string is SQL nvarchar (16 bit Unicode). This command changes the SQL column type to varchar (8 bit ASCII)
    #D I add an index to the PublishedOn property because I sort and filter on this property
    #E This sets a model-level query filter on the Book entity. By default, a query will exclude Book entites where th SoftDeleted property is true
     * * ******************************************************/
    /*CH07********************************************************
    #A This defines the One-to-One relationship to the promotion that a book can optionally have. The foreign key is in the PriceOffer
    #B This defines the One-to-Many relationship, with a book having zero to many reviews
     * ***********************************************************/
}