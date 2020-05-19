// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace Test.Chapter06Listings
{
    public class Person
    {

        public int PersonId { get; set; }

        public string Name { get; set; }

        //------------------------------------------------------
        //simple property with bakcing field

        private string _myProperty;

        public string MyProperty
        {
            get { return _myProperty; }
            set { _myProperty = value; }
        }

        //----------------------------------------------------
        //C#6 auto-property

        public int AutoProperty { get; private set; }

        public void SetAutoProperty(int value)
        {
            AutoProperty = value;
        }

        //-----------------------------------------------------
        //'private' backing field

        private DateTime _dateOfBirth; //#A

        public void SetDateOfBirth(DateTime dateOfBirth) //#B
        {
            _dateOfBirth = dateOfBirth;
        }

        public int AgeYears => //#C
            Years(_dateOfBirth, DateTime.Today);

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

        /**************************************************
        #A This is the private backing field. I cannot be directly accessed via normal .Net software.
        #B This method allows the backing field to be set
        #C I can access the person's age, but not their exact date of birth
         * **********************************************/

        //--------------------------------------------------
        //Backing field with transformation on get

        private DateTime _updatedOn;

        public DateTime UpdatedOn
        {
            get
            {
                return DateTime.SpecifyKind(
                    _updatedOn, DateTimeKind.Utc);
            }
            set { _updatedOn = value; }
        }
    }
}