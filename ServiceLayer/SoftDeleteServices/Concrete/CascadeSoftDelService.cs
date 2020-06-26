// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using ServiceLayer.SoftDeleteServices.Concrete.Internal;

namespace ServiceLayer.SoftDeleteServices.Concrete
{
    public enum CascadeSoftDelWhatDoing { SoftDelete, UnSoftDelete, CheckWhatWillDelete, HardDeleteSoftDeleted }

    public class CascadeSoftDelService
    {
        private readonly DbContext _context;


        /// <summary>
        /// This provides a equivalent to a SQL cascade delete, but using a soft delete approach.
        /// </summary>
        /// <param name="context"></param>
        public CascadeSoftDelService(DbContext context)
        {
            _context = context;
        }

        public CascadeSoftDeleteInfo SetCascadeSoftDelete<TEntity>(TEntity softDeleteThisEntity, bool readEveryTime = true)
            where TEntity : class
        {
            var walker = new CascadeWalker(_context, CascadeSoftDelWhatDoing.SoftDelete, readEveryTime);
            walker.AlterCascadeSoftDelete(softDeleteThisEntity, 1);
            _context.SaveChanges();
            return walker.Info;
        }

        public CascadeSoftDeleteInfo ResetCascadeSoftDelete<TEntity>(TEntity softDeleteThisEntity)
            where TEntity : class
        {
            //For reset you need to read every time because some of the collection might be soft deleted already
            var walker = new CascadeWalker(_context, CascadeSoftDelWhatDoing.UnSoftDelete, true);
            walker.AlterCascadeSoftDelete(softDeleteThisEntity, 1);
            _context.SaveChanges();
            return walker.Info;
        }

        


    }
}