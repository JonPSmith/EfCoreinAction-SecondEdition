// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Test.Chapter08Listings.EfClasses
{
    public class ShadowWithNotes
    {
        public int ShadowWithNotesId { get; set; }

        public ICollection<ShadowAttendeeNote> Notes { get; set; }
    }
}