// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Test.Chapter12Listings.EventInterfacesEtc;
using Test.Chapter12Listings.Events;

namespace Test.Chapter12Listings.EfClasses
{
    public class Location : AddEventsToEntity
    {
        private string _state;
        public int LocationId { get; set; }
        public string Name { get; set; }

        [MaxLength(20)]
        public string State
        {
            get => _state;
            set
            {
                if (value != _state)
                    AddEvent(new LocationChangedEvent(this, value));
                _state = value;
            } 
        }

        public override string ToString()
        {
            return $"Name: {Name}, State: {State}";
        }
    }
}