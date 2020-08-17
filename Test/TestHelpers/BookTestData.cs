// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;

namespace Test.TestHelpers
{
    public static class BookTestData
    {
        public static readonly DateTime DummyBookStartDate = new DateTime(2017, 1, 1);

        public static void SeedDatabaseDummyBooks(this BookDbContext context, int numBooks = 10, bool stepByYears = false)
        {
            context.Books.AddRange(CreateDummyBooks(numBooks, stepByYears));
            context.SaveChanges();
        }

        public static Book CreateDummyBookOneAuthor()
        {
            var book = Book.CreateBook
            (
                "Book Title",
                "Book Description",
                DummyBookStartDate, 
                "Book Publisher",
                123,
                null,
                new[] { new Author( "Test Author", "author@gmail.com") }
            );

            return book.Result;
        }

        public static Book CreateDummyBookTwoAuthorsTwoReviews()
        {
            var book = Book.CreateBook
            (
                "Book Title",
                "Book Description",
                DummyBookStartDate,
                "Book Publisher",
                123,
                null,
                new[]
                {
                    new Author( "Test Author1", "author1@gmail.com"),
                    new Author( "Test Author2", "author2@gmail.com")
                }
            );
            book.Result.AddReview(5, null, "test1");
            book.Result.AddReview(1, null, "test2");

            return book.Result;
        }

        public static List<Book> CreateDummyBooks(int numBooks = 10, bool stepByYears = false)
        {
            var result = new List<Book>();
            var commonAuthor = new Author("Common Author", "common@gmail.com");
            for (int i = 0; i < numBooks; i++)
            {
                var book = Book.CreateBook
                (
                    $"Book{i:D4} Title",
                    $"Book{i:D4} Description",
                    stepByYears ? DummyBookStartDate.AddYears(i) : DummyBookStartDate.AddDays(i),
                    "Publisher",
                    (short)(i + 1),
                    $"Image{i:D4}",
                    new[] { new Author( $"Author{i:D4}", $"author{i:D4}@gmail.com"), commonAuthor }
                ).Result;
                for (int j = 0; j < i; j++)
                {
                    book.AddReview((j % 5) + 1, null, j.ToString());
                }

                result.Add(book);
            }

            return result;
        }


        public static List<Book> SeedDatabaseFourBooks(this BookDbContext context)
        {
            var fourBooks = CreateFourBooks();
            context.Books.AddRange(fourBooks);
            context.SaveChanges();
            return fourBooks;
        }

        public static List<Book> CreateFourBooks()
        {
            var martinFowler = new Author("Martin Fowler", "mf@gmail.com");

            var books = new List<Book>();

            var book1 = Book.CreateBook
            (
                "Refactoring",
                "Improving the design of existing code",
                new DateTime(1999, 7, 8),
                null,
                40,
                null,
                new[] { martinFowler }
            ).Result;
            books.Add(book1);

            var book2 = Book.CreateBook
            (
                "Patterns of Enterprise Application Architecture",
                "Written in direct response to the stiff challenges",
                new DateTime(2002, 11, 15),
                null,
                53,
                null,
                new[] { martinFowler }
            ).Result;
            book2.AuthorsOrdered = martinFowler.Name;
            books.Add(book2);

            var book3 = Book.CreateBook
            (
                "Domain-Driven Design",
                 "Linking business needs to software design",
                 new DateTime(2003, 8, 30),
                 null,
                56,
                null,
                new[] { new Author("Eric Evans", "ee@gmail.com") }
            ).Result;
            books.Add(book3);

            var book4 = Book.CreateBook
            (
                "Quantum Networking",
                "Entangled quantum networking provides faster-than-light data communications",
                new DateTime(2057, 1, 1),
                "Future Published",
                220,
                null,
                new[] { new Author("Future Person", "fp@gmail.com") }
            ).Result;
            book4.AuthorsOrdered = book4.AuthorsLink.First().Author.Name;
            book4.AddReview(5,
                "I look forward to reading this book, if I am still alive!", "Jon P Smith");
            book4.AddReview(3,
                "I write this book if I was still alive!", "Albert Einstein"); book4.AddPromotion(219, "Save $1 if you order 40 years ahead!");
            book4.AddPromotion(219, "Save 1$ by buying 40 years ahead");
            books.Add(book4);

            return books;
        }
    }
}
