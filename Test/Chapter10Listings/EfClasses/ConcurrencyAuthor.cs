// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter10Listings.EfClasses
{
    public class ConcurrencyAuthor
    {
        public int ConcurrencyAuthorId { get; set; }

        public string Name { get; set; }

        [Timestamp] //#A
        public byte[] ChangeCheck { get; set; }
    }
    /***********************************************
    #A This marks the property ChangeCheck as as a timestamp. This will cause the database server to mark it as a SQL ROWVERSION and EF Core will check this when updating to see if this has changed
     * *********************************************/
}