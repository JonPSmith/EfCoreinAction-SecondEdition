// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace Test.Chapter12Listings.BusinessLogic
{
    public class OutOfStockException : Exception
    {
        public OutOfStockException(string message)
            : base(message) {}
    }
}