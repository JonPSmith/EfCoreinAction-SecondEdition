// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Chapter08Listings.EfClasses
{
    public enum Shapes { Rectangle, Bottle, Jar }

    public class PlasticContainer : Container //#D
    {
        public int CapacityMl { get; set; }    //#F
        public Shapes Shape { get; set; }      //#F
        public string ColorARGB { get; set; }  //#F
    }
}