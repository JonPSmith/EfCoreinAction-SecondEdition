// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.UnitTests.TestDataLayer
{
    public class Ch10_InMemory
    {
        //standard localdb is 2014, not 2016, so in-memory is not supported
        //[Fact]
        //public void TestAddOneOk()
        //{
        //    //SETUP
        //    var connection = this.GetUniqueDatabaseConnectionString();
        //    var optionsBuilder =
        //        new DbContextOptionsBuilder<Chapter10DbContext>();

        //    optionsBuilder.UseSqlServer(connection);
        //    using (var context = new Chapter10DbContext(optionsBuilder.Options))
        //    {
        //        context.Database.EnsureCreated();

        //        //ATTEMPT
        //        var entity = new InMemoryTest {TestCode = "1"};
        //        context.Add(entity);
        //        context.SaveChanges();

        //        //VERIFY
        //        entity.Id.ShouldNotEqual(0);
        //    }
        //}
    }
}