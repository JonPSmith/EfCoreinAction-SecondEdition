// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BookApp.Domain.Books
{
    public class Tag
    {
        private HashSet<Book> _books;

        public Tag(string tagId)
        {
            TagId = tagId;
        }

        [Key]
        [Required]
        [MaxLength(40)]
        public string TagId { get; private set; }

        public IReadOnlyCollection<Book> Books => _books.ToList();
    }
}