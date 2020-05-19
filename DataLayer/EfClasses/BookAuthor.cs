// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.EfClasses
{
    public class BookAuthor               
    {
        [Key] //#A
        [Column(Order = 0)] //#B
        public int BookId { get; set; }
        [Key] //#A
        [Column(Order = 1)]  //#B
        public int AuthorId { get; set; } 
        public byte Order { get; set; }   

        //-----------------------------
        //Relationships

        public Book Book { get; set; }      
        public Author Author { get; set; }  
    }
    /**************************************************
    #A The [Key] attribute tells EF Core that the property is a primary key
    #B The [Column(Order = nn)] tells EF Core the order in which the keys should appear in the composite key. Note: the numbers are relative - I could have used 100 and 200
     * ***********************************************/
}