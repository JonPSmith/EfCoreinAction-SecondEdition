// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter08Listings.EfClasses
{
    public abstract class Container          //#A
    {
        [Key]
        public int ContainerId { get; set; } //#B

        public int HeightMm { get; set; } //#C
        public int WidthMm { get; set; }  //#C
        public int DepthMm { get; set; }  //#C
    }
    /**********************************************************
    #A The Container class is marked as abstract because it won't be created
    #B This becomes the primary key for each TPT table
    #C Common part to each container is the overall height, width, depth
    #D The class inherits the Container class
    #E These properties are unique to a shipping container
    #F These properties are unique to a plastic container
     *******************************************************/
}