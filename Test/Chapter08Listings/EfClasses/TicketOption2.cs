// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter08Listings.EfClasses
{

    public class TicketOption2
    {
        [Key]
        public int TicketId { get; set; }

        public int AttendeeId { get; set; }

        [ForeignKey(nameof(AttendeeId))]
        public Attendee Attendee { get; set; }
    }
}