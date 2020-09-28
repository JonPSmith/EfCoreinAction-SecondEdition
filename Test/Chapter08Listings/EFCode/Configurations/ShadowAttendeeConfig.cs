// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter08Listings.EfClasses;

namespace Test.Chapter08Listings.EFCode.Configurations
{
    public class ShadowAttendeeConfig : IEntityTypeConfiguration<ShadowAttendee>
    {
        public void Configure
            (EntityTypeBuilder<ShadowAttendee> entity)
        {
            entity.HasOne(p => p.TicketOption1) 
                .WithOne(p => p.Attendee)
                .HasForeignKey<ShadowAttendee>();
            entity.HasOne(p => p.TicketOption2)
                .WithOne(p => p.Attendee)
                .HasForeignKey<TicketOption2>();
        }
    }
}