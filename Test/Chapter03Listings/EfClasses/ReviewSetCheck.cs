// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.


using System.ComponentModel.DataAnnotations;

namespace Test.Chapter03Listings.EfClasses
{
    public class ReviewSetCheck
    {
        [Key]
        public int ReviewId { get; set; }
        public string VoterName { get; set; }
        public int NumStars { get; set; }
        public string Comment { get; set; }

        //-----------------------------------------
        //Relationships

        public int BookId { get; set; } //#M
    }
}