// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Chapter08Listings.EfClasses
{
    public class Attendee
    {
        public int AttendeeId { get; set; }
        public string Name { get; set; }

        public int TicketId { get; set; } //#A
        public TicketOption1 TicketOption1 { get; set; }//#B
        public TicketOption2 TicketOption2 { get; set; }//Not for listing, only for checking
        public TicketOption3 TicketOption3 { get; set; }//Not for listing, only for checking 

        public OptionalTrack Optional { get; set; } //#C
        public RequiredTrack Required { get; set; } //#D
    }
    /*******************************************************
    #A This is the foreign key for the one-to-one relationship, TicketOption1
    #B This is the one-to-one navigational property that accesses the TicketOption1 entity
    #C This is a one-to-one navigational property using a shadow property for the foreign key. By default, the foreign key is nullable, so the relationship is optional
    #D This is a one-to-one navigational property using a shadow property for the foreign key. I use Fluent API commands to say that the foreign key is NOT nullable, so the relationship is required
     * *****************************************************/
}