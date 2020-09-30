// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Test.Chapter08Listings.PropertyBags
{
    public class TableSpec
    {
        public TableSpec(string name, List<PropertySpec> properties)
        {
            Name = name;
            Properties = properties;
        }

        public string Name { get; }
        public List<PropertySpec> Properties { get; }
    }
}