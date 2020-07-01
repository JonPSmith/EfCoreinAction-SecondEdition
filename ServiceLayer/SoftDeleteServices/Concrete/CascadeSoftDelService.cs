// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.SoftDeleteServices.Concrete.Internal;

namespace ServiceLayer.SoftDeleteServices.Concrete
{
    public enum CascadeSoftDelWhatDoing { SoftDelete, ResetSoftDelete, CheckWhatWillDelete, HardDeleteSoftDeleted }

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
            where TEntity : class, ICascadeSoftDelete
        {
            //If is a one-to-one entity we return an error
            var keys = _context.Entry(softDeleteThisEntity).Metadata.GetForeignKeys();
            if (!keys.All(x => x.DependentToPrincipal?.IsCollection == true || x.PrincipalToDependent?.IsCollection == true))
                //This it is a one-to-one entity
                return new CascadeSoftDeleteInfo(CascadeSoftDelWhatDoing.SoftDelete, "You cannot soft delete a one-to-one relationship");

            var walker = new CascadeWalker(_context, CascadeSoftDelWhatDoing.SoftDelete, readEveryTime);
            walker.WalkEntitiesSoftDelete(softDeleteThisEntity, 1);
            _context.SaveChanges();
            return walker.Info;
        }

        public CascadeSoftDeleteInfo ResetCascadeSoftDelete<TEntity>(TEntity resetSoftDeleteThisEntity)
            where TEntity : class, ICascadeSoftDelete
        {
            if (resetSoftDeleteThisEntity.SoftDeleteLevel == 0)
                return new CascadeSoftDeleteInfo(CascadeSoftDelWhatDoing.ResetSoftDelete, "This entry isn't soft deleted");

            if (resetSoftDeleteThisEntity.SoftDeleteLevel > 1)
                return new CascadeSoftDeleteInfo(CascadeSoftDelWhatDoing.ResetSoftDelete, 
                    $"This entry was soft deleted {resetSoftDeleteThisEntity.SoftDeleteLevel - 1} " +
                    $"level{(resetSoftDeleteThisEntity.SoftDeleteLevel > 2  ? "s" : "")} above here");

            //For reset you need to read every time because some of the collection might be soft deleted already
            var walker = new CascadeWalker(_context, CascadeSoftDelWhatDoing.ResetSoftDelete, true);
            walker.WalkEntitiesSoftDelete(resetSoftDeleteThisEntity, 1);
            _context.SaveChanges();
            return walker.Info;
        }

        public CascadeSoftDeleteInfo CheckCascadeSoftDelete<TEntity>(TEntity checkHardDeleteThisEntity)
            where TEntity : class, ICascadeSoftDelete
        {
            //For reset you need to read every time because some of the collection might be soft deleted already
            var walker = new CascadeWalker(_context, CascadeSoftDelWhatDoing.CheckWhatWillDelete, true);
            walker.WalkEntitiesSoftDelete(checkHardDeleteThisEntity, 1);
            return walker.Info;
        }


        public CascadeSoftDeleteInfo HardDeleteSoftDeletedEntries<TEntity>(TEntity hardDeleteThisEntity)
            where TEntity : class, ICascadeSoftDelete
        {
            //For reset you need to read every time because some of the collection might be soft deleted already
            var walker = new CascadeWalker(_context, CascadeSoftDelWhatDoing.HardDeleteSoftDeleted, true);
            walker.WalkEntitiesSoftDelete(hardDeleteThisEntity, 1);
            _context.SaveChanges();
            return walker.Info;
        }


    }
}