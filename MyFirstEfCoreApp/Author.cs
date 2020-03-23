// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

namespace MyFirstEfCoreApp
{
    public class Author
    {
        public int AuthorId { get; set; }    //#D
        public string Name { get; set; }
        public string WebUrl { get; set; }
    }
    /*******************************************************
    #D This holds the Primary Key of the Author row in the database
     * ************************************************/
}