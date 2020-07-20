// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace Test.Chapter10Listings.EfCode
{
    public class MyUdfMethods
    {
        public static double? AverageVotes(int id) 
        {
            throw new NotImplementedException(
                "Called in Client vs. Server evaluation.");
        }
    }
}