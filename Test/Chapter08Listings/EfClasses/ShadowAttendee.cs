// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Test.Chapter08Listings.EfClasses
{
    public class ShadowAttendee
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public TicketOption1 TicketOption1 { get; set; }
        public TicketOption2 TicketOption2 { get; set; }

        public ICollection<ShadowAttendeeNote> Notes { get; set; }
    }
}