// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter08Listings.EfClasses
{
    [Table("ShippingContainer")]
    public class ShippingContainer : Container
    {
        public int ThicknessMm { get; set; }
        public string DoorType { get; set; }
        public int StackingMax { get; set; }
        public bool Refrigerated { get; set; }
    }
}