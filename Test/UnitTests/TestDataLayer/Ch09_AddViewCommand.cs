// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter09Listings.AddViewCommand;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch09_AddViewCommand
    {
        private readonly ITestOutputHelper _output;

        public Ch09_AddViewCommand(ITestOutputHelper output)
        {
            _output = output;
        }

        [RunnableInDebugOnly]
        public void TestMigrateAddViewOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<AddViewCommandDbContext>();
            using (var context = new AddViewCommandDbContext(options))
            {
                context.Database.EnsureDeleted();
                
                context.Database.Migrate();

                var list = new List<MyEntity>();
                for (int i = 0; i < 5; i++)
                {
                    list.Add(new MyEntity { MyDateTime = new DateTime(2000 + i*10, 1, 1), MyString = (2000 + i * 10).ToString() });
                }
                context.AddRange(list);
                context.SaveChanges();

                //ATTEMPT
                var result = context.MyViews.ToList();

                //VERIFY
                result.Count.ShouldEqual(3);
            }
        }

    }
}