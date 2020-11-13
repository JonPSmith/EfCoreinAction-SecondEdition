// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace BookApp.Domain.Books
{
    public class CosmosBook 
    {
        public int BookId { get; set; }  //#A

        public string Title { get; set; }           //#B
        public DateTime PublishedOn { get; set; }   //#B
        public bool EstimatedDate { get;  set; }    //#B
        public int YearPublished { get; set; }      //#B
        public decimal OrgPrice { get; set; }       //#B
        public decimal ActualPrice { get; set; }    //#B
        public string PromotionalText { get; set; } //#B
        public string ManningBookUrl { get; set; }  //#B

        public string AuthorsOrdered { get; set; }       //#C
        public int ReviewsCount { get; set; }            //#C
        public double? ReviewsAverageVotes { get; set; } //#C

        public List<CosmosTag> Tags { get; set; }  //#D

        public string TagsString { get; set; }  //#E

        public override string ToString()
        {
            return $"{Title}: by {AuthorsOrdered}. Price {ActualPrice}, {ReviewsCount} reviews. Published {PublishedOn:d}, Tags: {TagsString}";
        }
    }
    /*********************************************************
    #A We use the BookId used in the SQL database to link this entity to the SQL entity
    #B These are normal properties that are needed to display the book
    #C These are pre-calculated values used for display and filtering
    #D To allow filtering on Tags we provide a list of CosmosTags, which are configured as Owned Types
    #E EF Core 5 doesn't support IN for Cosmos DB, but it does support string contains
     ************************************************************/

}