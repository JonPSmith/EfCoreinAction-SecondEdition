// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace BookApp.ServiceLayer.DefaultSql.Books.Dtos
{
    public class BookListDto
    {
        public int BookId { get; set; } 
        public string Title { get; set; }
        public DateTime PublishedOn { get; set; } 
        public bool EstimatedDate { get; set; }
        public decimal OrgPrice { get; set; } 

        public decimal ActualPrice { get; set; } 

        public string PromotionText { get; set; } 

        public string AuthorsOrdered { get; set; } 

        public int ReviewsCount { get; set; } 

        public double? ReviewsAverageVotes { get; set; } 

        /******************************************************
        #A I need the Primary Key if the customer clicks the entry to buy the book
        #B While the publish date isn't shown we will want to sort by it, so we have to include it
        #C This is the normal OrgPrice
        #D This is the selling price - either the normal price, or the promotional.NewPrice if present
        #E The promotional text to show if there is a new price
        #F An array of the authors' names in the right order
        #G The number of people who reviewed the book
        #H The average of all the Votes - null if no votes
         * ***************************************************/
    }
}