// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter08Listings.EfClasses;

namespace Test.Chapter08Listings.EFCode.Configurations
{
    public class Ch07BookConfig : IEntityTypeConfiguration<Ch08Book>
    {
        public void Configure
            (EntityTypeBuilder<Ch08Book> entity)
        {
            entity.HasKey(p => p.BookId);

            //entity.HasMany(x => x.Reviews)
            //    .WithOne()
            //    .Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);

        }

        /******************************************************
        #A Using the MetaData for this entity class I can access some of the deeper features of the entity class
        #B This finds the navigation property using the name of the property
        #C This sets the access mode so that EF Core will ONLY read/write to the backing field
         * ****************************************************/
    }
}