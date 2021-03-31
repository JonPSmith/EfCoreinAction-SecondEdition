// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace BookApp.ServiceLayer.UdfsSql.Books.Dtos
{
    public class UdfsBookListDto
    {
        public int BookId { get; set; } 
        public string Title { get; set; }
        public DateTime PublishedOn { get; set; } 
        public bool EstimatedDate { get; set; }
        public decimal OrgPrice { get; set; } 

        public decimal ActualPrice { get; set; } 

        public string PromotionText { get; set; } 

        public string AuthorsOrdered { get; set; } 

        public string TagsString { get; set; }

        public int ReviewsCount { get; set; } 

        public double? ReviewsAverageVotes { get; set; }

        public string ManningBookUrl { get; set; }
    }
}