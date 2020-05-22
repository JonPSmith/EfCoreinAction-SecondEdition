// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter07Listings
{
    public class Person
    {

        public int PersonId { get; set; }

        public string Name { get; set; }

        //------------------------------------------------------
        //simple property with backing field

        private string _myProperty;

        public string MyProperty
        {
            get { return _myProperty; }
            set { _myProperty = value; }
        }

        //-----------------------------------------------------
        //backing field via Data Annotations

        private string _fieldName1;

        [BackingField(nameof(_fieldName1))]
        public string BackingFieldViaAnnotation
        {
            get { return _fieldName1; }
        }

        public void SetPropertyAnnotationValue(string someString)
        {
            _fieldName1 = someString;
        }

        //-----------------------------------------------------
        //backing field via Fluent API

        private string _fieldName2;

        public string BackingFieldViaFluentApi => _fieldName2;

        public void SetPropertyFluentValue(string someString)
        {
            _fieldName2 = someString;
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

        public int AutoProperty { get; private set; }

        public void SetAutoProperty(int value)
        {
            AutoProperty = value;
        }
    }
}