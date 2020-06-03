// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Chapter08Listings.EfClasses
{
    public enum PTypes : byte {  Cash = 1, Card = 2}
    public abstract class Payment
    {
        public int PaymentId { get; set; }

        public PTypes PType { get; set; }

        public decimal Amount { get; set; }
    }
}