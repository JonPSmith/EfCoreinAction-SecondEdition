// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Chapter09Listings.SeedExample
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }

        public SimpleAddress Address { get; set; }

        public int ProjectId { get; set; }


        public override string ToString()
        {
            return $"{nameof(UserId)}: {UserId}, {nameof(Name)}: {Name}, {Address}, {nameof(ProjectId)}: {ProjectId}";
        }
    }
}