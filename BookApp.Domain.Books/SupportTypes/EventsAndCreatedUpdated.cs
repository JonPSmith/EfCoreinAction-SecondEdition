// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using GenericEventRunner.DomainParts;

namespace BookApp.Domain.Books.SupportTypes
{
    public class EventsAndCreatedUpdated : EntityEventsBase, ICreatedUpdated
    {
        public DateTime WhenCreatedUtc { get; private set; }
        public DateTime LastUpdatedUtc { get; private set; }
        public bool NotUpdatedYet => WhenCreatedUtc == LastUpdatedUtc;

        public void LogAddUpdate(bool added)
        {
            var timeNow = DateTime.UtcNow;
            if (added)
                WhenCreatedUtc = timeNow;
            LastUpdatedUtc = timeNow;
        }
    }
}