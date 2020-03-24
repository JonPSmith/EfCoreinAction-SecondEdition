// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace ServiceLayer.DatabaseServices.Concrete
{
    public class DemoSetupOptions
    {
        public bool
            UseInMemory { get; set; } //If true it a Sqlite in-memory database, otherwise it uses a SQL server database.

        public bool
            Migrate
        {
            get;
            set;
        } //If true it will run a migration on the database, otherwise it will use context.Database.EnsureCreated to create the database

        public bool
            ManuallySeed
        {
            get;
            set;
        } //If this is true then it will create the database and manually seed the database of no books are already in it.
    }
}