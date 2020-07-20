// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace Test.Chapter10Listings.EfClasses
{
    public class Person
    {
        private DateTime _dateOfBirth;

        public int PersonId { get; set; }
        public string Name { get; set; }
        public int YearOfBirth { get; private set; }  //#A

        public int AgeYears => //#C
            Years(_dateOfBirth, DateTime.Today);


        public void SetDateOfBirth(DateTime dateOfBirth) //#B
        {
            _dateOfBirth = dateOfBirth;
        }

        //Thanks to dana on stackoverflow
        //see http://stackoverflow.com/a/4127477/1434764
        private static int Years(DateTime start, DateTime end)
        {
            return (end.Year - start.Year - 1) +
                   (((end.Month > start.Month) ||
                     ((end.Month == start.Month)
                      && (end.Day >= start.Day)))
                       ? 1 : 0);
        }
    }
}