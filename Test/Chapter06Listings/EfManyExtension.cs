// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Test.Chapter06Listings
{
    public static class EfManyExtension
    {
        public static ManyTop AddManyTopWithRelationsToDb(this Chapter06Context context, int numRelations = 100)
        {
            var manyTop = new ManyTop();
            manyTop.Collection1 = new List<Many1>();
            manyTop.Collection2 = new List<Many2>();
            manyTop.Collection3 = new List<Many3>();
            for (int i = 0; i < numRelations; i++)
            {
                manyTop.Collection1.Add(new Many1());
                manyTop.Collection2.Add(new Many2());
                manyTop.Collection3.Add(new Many3());
            }

            context.Add(manyTop);
            context.SaveChanges();

            return manyTop;
        }
    }
}