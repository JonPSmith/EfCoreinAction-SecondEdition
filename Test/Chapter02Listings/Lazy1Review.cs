// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Chapter02Listings
{
    public class Lazy1Review 
    {
        public int Id { get; set; }
        public int NumStars { get; set; }

        //-----------------------------------------
        //Relationships

        //I don't place a foreign key here, which means EF Core will provide a foreign key via shadow properties.
    }


}