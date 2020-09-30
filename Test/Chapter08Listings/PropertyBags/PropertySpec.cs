// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace Test.Chapter08Listings.PropertyBags
{
    public class PropertySpec
    {
        public PropertySpec(string name, Type propType, bool addRequired = false)
        {
            Name = name;
            PropType = propType;
            AddRequired = addRequired;
        }

        public string Name { get; }
        public Type PropType { get; }
        public bool AddRequired { get; }
    }
}