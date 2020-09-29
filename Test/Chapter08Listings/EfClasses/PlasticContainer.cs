// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter08Listings.EfClasses
{
    public enum Shapes { Rectangle, Bottle, Jar }

    [Table("PlasticContainer")]
    public class PlasticContainer : Container
    {


        public int CapacityMl { get; set; }
        public Shapes Shape { get; set; }
        public string ColorARGB { get; set; }
    }
}