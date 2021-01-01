// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter11Listings.EfClasses
{
    public class BookSqlQuery
    {
        [Key]
        public int BookId { get; set; }
        public string Title { get; set; }
        
        public double? AverageVotes { get; set; }
    }
}