// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;

namespace Test.UnitTests.TestSupportCode
{
    public static class LinqHelpers //#A
    {
        public static IQueryable<int> MyOrder //#B
        (this IQueryable<int> queryable, //#C
            bool ascending) //#D
        {
            return ascending //#E
                ? queryable //#F
                    .OrderBy(num => num) //#F
                : queryable //#G
                    .OrderByDescending(num => num); //#G
        }
    }

    /*******************************************************
    #A An extension method needs to be defined in a static class
    #B The static method Order returns an IQueryable<int>, so that other extension methods can 'chain' on
    #C The extension method's first parameter is a) of IQueryable, and b) starts with the 'this' keyword
    #D I provide a second parameter that allows me to change the order of the sorting
    #E I use the boolean parameter ascending to control whether I add the OrderBy or OrderByDescending LINQ operator to the IQueryable result
    #F The ascending parameter was true, so I add the OrderBy LINQ operator to the IQueryable input
    #G The ascending parameter was false, so I add the OrderByDescending LINQ operator to the IQueryable input
     * *****************************************************/
}