// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace DataLayer.EfClasses
{
    public class BookAuthor //#G
    {
        public int BookId { get; set; } //#H
        public int AuthorId { get; set; } //#H
        public byte Order { get; set; } //#I

        //-----------------------------
        //Relationships

        public Book Book { get; set; } //#J
        public Author Author { get; set; } //#K
    }

    /**************************************************
    #G The BookAuthor class is the Many-to-Many linking table between the Books and Authors tables
    #H The Primary Key is made up of the two keys of the Book and Author
    #I The order of the Authors in a book matters, so I use this to set the right order
    #J This is the link to the Book side of the relationship
    #K And this links to the Author side of the relationship
     * ***********************************************/
}