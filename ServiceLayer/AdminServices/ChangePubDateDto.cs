// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.AdminServices
{
    public class ChangePubDateDto
    {
        public int BookId { get; set; }          //#A
        public string Title { get; set; }        //#B

        [DataType(DataType.Date)]                //#C
        public DateTime PublishedOn { get; set; }//#C
    }
    /*************************************************
    #A This holds the primary key of the row we want to update. This makes finding the right entry quick and accurate
    #B We send over the title to show the user, so that they can be sure they are altering the right book
    #C This is the property we want to alter. We send out the current publication date and get back the changed publication date
     * ********************************************/
}