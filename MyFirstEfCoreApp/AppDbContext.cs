// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace MyFirstEfCoreApp
{
    public class AppDbContext : DbContext
    {
        private const string ConnectionString = //#A
            @"Server=(localdb)\mssqllocaldb;
             Database=MyFirstEfCoreDb;
             Trusted_Connection=True";

        public DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(
            DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString); //#B
        }
    }

    /********************************************************
    #A The connection string is used by the SQL Server database provider to find the database
    #B Using the SQL Server database provider’s UseSqlServer command sets up the options ready for creating the applications’s DBContext
     ********************************************************/
}