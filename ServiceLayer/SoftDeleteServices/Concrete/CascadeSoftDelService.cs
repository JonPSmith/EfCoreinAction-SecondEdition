// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.SoftDeleteServices.Concrete.Internal;

namespace ServiceLayer.SoftDeleteServices.Concrete
{
    public class CascadeSoftDelService
    {
        private readonly DbContext _context;


        /// <summary>
        /// This provides a equivalent to a SQL cascade delete, but using a soft delete approach.
        /// </summary>
        /// <param name="context"></param>
        /// But taking advantage of collections not loaded being null you can set this to false and save loading already loaded collections</param>
        public CascadeSoftDelService(DbContext context)
        {
            _context = context;
        }

        public int SetCascadeSoftDelete<TEntity>(TEntity softDeleteThisEntity, bool readEveryTime = true)
            where TEntity : class
        {
            var walker = new CascadeWalker(_context,  true, readEveryTime);
            walker.AlterCascadeSoftDelete(softDeleteThisEntity, 1);
            _context.SaveChanges();
            return walker.NumChanged;
        }

        public int ResetCascadeSoftDelete<TEntity>(TEntity softDeleteThisEntity)
            where TEntity : class
        {
            //For reset you need to read avery time because some of the collection might be soft deleted already
            var walker = new CascadeWalker(_context, false, true);
            walker.AlterCascadeSoftDelete(softDeleteThisEntity, 1);
            _context.SaveChanges();
            return walker.NumChanged;
        }

        


    }
}