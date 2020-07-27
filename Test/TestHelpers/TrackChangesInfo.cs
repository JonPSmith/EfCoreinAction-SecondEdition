// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Test.TestHelpers
{
    public static class TrackChangesInfo
    {
        public static EntityState GetEntityState(this DbContext context, object entity)
        {
            return context.Entry(entity).State;
        }

        public static bool GetEntityIsKeySet(this DbContext context, object entity)
        {
            return context.Entry(entity).IsKeySet;
        }

        public static IReadOnlyList<IProperty> GetEntityPrimaryKeys<TEntity>(this DbContext context, TEntity entity)
        {
            return context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties;
        }

        public static int NumTrackedEntities(this DbContext context)
        {
            return context.ChangeTracker.Entries().Count();
        }

        public static string GetAllPropsNavsIsModified(this DbContext context, object entity)
        {
            var entityEntry = context.Entry(entity);
            var isModifiedNames = entityEntry.Properties.Where(p => p.IsModified).Select(x => x.Metadata.Name)
                .Union(entityEntry.Navigations.Where(p => p.IsModified).Select(x => x.Metadata.Name));
            return string.Join(",", isModifiedNames);
        }


        public static bool GetPropertyIsModified<TEntity, TProperty>(this DbContext context, TEntity entity, 
            Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var propInfo = DatabaseMetadata.GetPropertyInfoFromLambda(model);
            var entityEntry = context.Entry(entity);
            return entityEntry.Property(propInfo.Name).IsModified;
        }

        public static bool GetNavigationalIsModified<TEntity, TProperty>(this DbContext context, TEntity entity,
            Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var propInfo = DatabaseMetadata.GetPropertyInfoFromLambda(model);
            var entityEntry = context.Entry(entity);
            var navData = entityEntry.Navigation(propInfo.Name);
            return navData.IsModified;
        }
    }
}