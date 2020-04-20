// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Test.TestHelpers;

namespace Test.Chapter05Listings
{
    public static class ExampleSeed
    {
        public static async Task SeedDatabaseAsync //#A
            (this EfCoreContext context)  //#A
        {
            if (context.Books.Any()) return;//#B

            context.Books.AddRange(           //#C
                EfTestData.CreateFourBooks());//#C
            await context.SaveChangesAsync(); //#D
        }

        /************************************************
        #A This is an extension method that takes in the application's DbContext
        #B If there are existing books I return, as I don't need to add any
        #C The database has no books, so I seed it, which in this case I add the default books
        #D SaveChangesAsync is called to update the database
         * ************************************************/
    }
}