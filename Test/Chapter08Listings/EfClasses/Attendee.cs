// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Test.Chapter08Listings.EfClasses
{
    public class Attendee
    {
        public int AttendeeId { get; set; }
        public string Name { get; set; }
        
        public OptionalTrack Optional { get; set; } //#B
        public RequiredTrack Required { get; set; } //#C
    }
    /*******************************************************
    #A This is the foreign key for the one-to-one relationship, TicketOption1
    #B This is a one-to-one navigational property using a shadow property for the foreign key. By default, the foreign key is nullable, so the relationship is optional
    #C This is a one-to-one navigational property using a shadow property for the foreign key. I use Fluent API commands to say that the foreign key is NOT nullable, so the relationship is required
     * *****************************************************/
}