// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Test")]

namespace BookApp.Domain.Books
{
    public class Review
    {
        public const int NameLength = 100;

        private Review() { }

        internal Review(int numStars, string comment, string voterName, Guid bookId = default(Guid))
        {
            NumStars = numStars;
            Comment = comment;
            VoterName = voterName;
            if (bookId != default(Guid))
                BookId = bookId;
        }

        [Key]
        public int ReviewId { get; private set; }

        [MaxLength(NameLength)]
        public string VoterName { get; private set; }

        public int NumStars { get; private set; }
        public string Comment { get; private set; }

        //-----------------------------------------
        //Relationships

        public Guid BookId { get; private set; }
    }

}