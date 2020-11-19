// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Reflection;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TestSupport.Attributes;
using TestSupport.Helpers;
using TestSupportSchema;

namespace Test.UnitCommands
{
    public class ResetAzureSqlDatabase
    {
        [RunnableInDebugOnly]
        public void ResetDatabaseOk()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(TestData.GetCallingAssemblyTopLevelDir())
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets(Assembly.GetAssembly( typeof(Startup)))
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config["AzureSql:Connection"];
            var builder = new DbContextOptionsBuilder<BookDbContext>();
            builder.UseSqlServer(connectionString);

            using var context = new BookDbContext(builder.Options);
            context.Database.EnsureClean(false);
        }
    }
}