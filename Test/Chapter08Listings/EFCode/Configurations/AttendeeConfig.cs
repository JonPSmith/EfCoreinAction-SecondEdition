// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter08Listings.EfClasses;

namespace Test.Chapter08Listings.EFCode.Configurations
{
    public class AttendeeConfig : IEntityTypeConfiguration<Attendee>
    {
        public void Configure
            (EntityTypeBuilder<Attendee> entity)
        {
            entity.HasOne(p => p.TicketOption1) //#A
                .WithOne(p => p.Attendee)
                .HasForeignKey<Attendee>
                    (p => p.TicketId) //#B
                .IsRequired();

            entity.HasOne(p => p.Optional)
                .WithOne(p => p.Attend)
                .HasForeignKey<Attendee>("OptionalTrackId");

            entity.HasOne(p => p.Required) //#C
                .WithOne(p => p.Attend)
                .HasForeignKey<Attendee>(
                    "MyShadowFk") //#D
                .IsRequired(); //#E
        }

        /*******************************************************************
        #A This sets up the one-to-one navigational relationship, TicketOption1, which has a foreign key defined in the Attendee class
        #B Here I specify the property that is the foreign key. Note how I need to provide the class type, as the foreign key could be in the principal or dependent entity class
        #C This sets up the one-to-one navigational relationship, Required, which does not have a foreign key defined for it
        #D I use the HasForeignKey<T> method that takes a string, because it is a shadow property and can only be referred to via a name. Note that I use my own name.
        #E In this case I use IsRequired to say the foreign key should not be nullable
         * ********************************************************************/
    }
}