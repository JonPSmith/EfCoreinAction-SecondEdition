// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter08Listings.EfClasses
{
    public class ShippingContainer : Container //#D
    {
        public int ThicknessMm { get; set; }   //#E
        public string DoorType { get; set; }   //#E
        public int StackingMax { get; set; }   //#E
        public bool Refrigerated { get; set; } //#E
    }
}