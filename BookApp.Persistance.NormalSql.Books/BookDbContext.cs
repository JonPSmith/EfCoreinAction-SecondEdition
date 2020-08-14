// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Reflection;
using BookApp.Domain.Books;
using BookApp.Domain.Books.SupportTypes;
using BookApp.Persistence.Common;
using GenericEventRunner.ForDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BookApp.Persistence.NormalSql.Books
{
    public class BookDbContext : DbContextWithEvents<BookDbContext>
    {

        public BookDbContext(DbContextOptions<BookDbContext> options, IEventsRunner eventRunner = null)
            : base(options, eventRunner)
        { }

        public DbSet<Book> Books { get; set; }                        
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AutoConfigureTypes();
            modelBuilder.AutoConfigureQueryFilters<BookDbContext>(this);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
