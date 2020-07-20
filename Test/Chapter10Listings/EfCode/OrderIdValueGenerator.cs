// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Test.Chapter10Listings.EfClasses;

namespace Test.Chapter10Listings.EfCode
{
    public class OrderIdValueGenerator 
        : ValueGenerator<string> // #A
    {
        public override bool 
            GeneratesTemporaryValues => false; //#B

        public override string Next //#C
            (EntityEntry entry)
        {
            var name = entry. //#D
                Property(nameof(DefaultTest.Name)) //#E
                    .CurrentValue;
            var uniqueNum = DateTime.UtcNow.Ticks;
            return $"{name}-{uniqueNum}"; //#F
        }
    }
    /**********************************************************
    #A My value generator needs to inherit from EF Core's ValueGenerator<T>
    #B Set this to false if you want your value to be written to the database
    #C This method is called when you Add the entity to the DbContext
    #D The parameter gives you access to the entity that the value generator is creating a value for. You can access its properties etc.
    #E I select the property called "Name" and get its current value
    #F You need to return a value of the Type you have defined at T in the inherted ValueGenerator<T> 
        * ********************************************************/
}
