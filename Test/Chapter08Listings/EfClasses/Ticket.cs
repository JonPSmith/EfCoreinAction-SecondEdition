// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Chapter08Listings.EfClasses
{

    public class Ticket
    {
        public enum TicketTypes : byte { Guest = 0, VIP = 1, Staff = 3}

        public int TicketId { get; set; }
        public TicketTypes TicketType { get; set; }

        public Attendee Attendee { get; set; }
    }
}