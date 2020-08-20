// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BookApp.Domain.Books;
using GenericServices;
using Microsoft.EntityFrameworkCore;

namespace BookApp.ServiceLayer.DefaultSql.Books.Dtos
{
    public class CreateBookDto : ILinkToEntity<Book>
    {
        public CreateBookDto()
        {
            PublishedOn = DateTime.Today;
        }

        //This will be populated with the primary key of the created book
        public int BookId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime PublishedOn { get; set; }
        public bool EstimatedDate { get; set; }
        public string Publisher { get; set; }

        [Range(0,1000)]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public ICollection<Author> Authors { get; set; }

        public ICollection<Tag> Tags { get; set; }

        //---------------------------------------------
        //Now the other parts used in the page where you enter data

        public List<KeyName> AllPossibleAuthors { get; private set; }

        public List<int> BookAuthorIds { get; set; } = new List<int>();

        public void BeforeDisplay(DbContext context)
        {
            AllPossibleAuthors = context.Set<Author>().Select(x => new KeyName(x.AuthorId, x.Name))
                .OrderBy(x => x.Name).ToList();
        }

        public void BeforeSave(DbContext context)
        {
            Authors = BookAuthorIds.Select(x => context.Find<Author>(x)).Where(x => x != null).ToList();
        }

        //---------------------------------------------------------
        //Now the data for the front end

        public struct KeyName
        {
            public KeyName(int authorId, string name)
            {
                AuthorId = authorId;
                Name = name;
            }

            public int AuthorId { get; }
            public string Name { get; }
        }
    }
}