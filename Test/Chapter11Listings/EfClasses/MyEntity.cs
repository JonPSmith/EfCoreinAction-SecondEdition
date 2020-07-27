// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Test.Chapter11Listings.EfClasses
{
    public class MyEntity
    {
        public int Id { get; set; }

        public string MyString { get; set; }

        public OneEntityOptional OneToOneOptional { get; set; }

        public OneEntityGuidOptional OneEntityGuidOptional { get; set; }

        public OneEntityRequired OneEntityRequired { get; set; }

        //Note: I don't normally use the form of collection, but I used it so that this entity is set up the same as the NotifyOne entity
        public ICollection<ManyEntity> Many { get; } = new HashSet<ManyEntity>();
    }
}