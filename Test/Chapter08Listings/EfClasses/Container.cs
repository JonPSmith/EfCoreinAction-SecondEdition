// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter08Listings.EfClasses
{
    public abstract class Container
    {
        [Key]
        public int ContainerId { get; set; }

        public int HeightMm { get; set; }
        public int WidthMm { get; set; }
        public int DepthMm { get; set; }
    }
}