﻿// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
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
                DummyBookStartDate, false, 
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
                DummyBookStartDate, false,
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
                    stepByYears ? DummyBookStartDate.AddYears(i) : DummyBookStartDate.AddDays(i), false,
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
            var editorsChoice = new Tag("Editor's Choice");
            var architectureTag = new Tag("Architecture");
            var refactoring = new Tag("Refactoring");

            var martinFowler = new Author("Martin Fowler", "mf@gmail.com");

            var books = new List<Book>();

            var book1 = Book.CreateBook
            (
                "Refactoring",
                new DateTime(1999, 7, 8), false,
                null,
                40,
                null,
                new[] { martinFowler },
                new List<Tag> { refactoring, editorsChoice }
            ).Result;
            books.Add(book1);

            var book2 = Book.CreateBook
            (
                "Patterns of Enterprise Application Architecture",
                new DateTime(2002, 11, 15), false,
                null,
                53,
                null,
                new[] { martinFowler },
                new List<Tag> { architectureTag }
            ).Result;
            books.Add(book2);

            var book3 = Book.CreateBook
            (
                "Domain-Driven Design",
                 new DateTime(2003, 8, 30), false,
                 null,
                56,
                null,
                new[] { new Author("Eric Evans", "ee@gmail.com") },
                new List<Tag> { architectureTag, editorsChoice }
            ).Result;
            books.Add(book3);

            var book4 = Book.CreateBook
            (
                "Quantum Networking",
                new DateTime(2057, 1, 1), false,
                "Future Published",
                220,
                null,
                new[] { new Author("Future Person", "fp@gmail.com") },
                new List<Tag> { new Tag("Quantum Entanglement") }
            ).Result;
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
