// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Chapter08Listings.EfClasses
{
    public class RequiredTrack
    {
        public int RequiredTrackId { get; set; }

        public TrackNames Track { get; set; }

        public Attendee Attend { get; set; }
    }
}