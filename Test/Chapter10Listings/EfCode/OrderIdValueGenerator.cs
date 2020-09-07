// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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
            GeneratesTemporaryValues => false;            //#B

        public override string Next                       //#C
            (EntityEntry entry)
        {
            var name = entry.                             //#D
                Property(nameof(DefaultTest.Name))        //#E
                    .CurrentValue;
            var ticks = DateTime.UtcNow.ToString("s");    //#F
            var guidString = Guid.NewGuid().ToString();   //#G
            var orderId = $"{name}-{ticks}-{guidString}"; //#H
            return orderId;                               //#I
        }
    }
    /**********************************************************
    #A The value generator needs to inherit from EF Core's ValueGenerator<T>
    #B Set this to false if you want your value to be written to the database
    #C This method is called when you Add the entity to the DbContext
    #D The parameter gives you access to the entity that the value generator is creating a value for. You can access its properties etc.
    #E I select the property called "Name" and get its current value
    #F This provides the date in a sortable format
    #G This provides a unique string 
    #H The orderId combines these three parts to create a unique orderId with useful info in it 
    #I The method must return a value of the Type you have defined at T in the inherited ValueGenerator<T> 
        * ********************************************************/
}
