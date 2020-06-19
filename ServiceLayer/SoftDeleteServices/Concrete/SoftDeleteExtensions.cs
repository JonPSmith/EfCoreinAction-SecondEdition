// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ServiceLayer.SoftDeleteServices.Concrete
{
    public static class SoftDeleteExtensions
    {
        public static void CascadeSoftDelete<TEntity>(this DbContext context, TEntity softDeleteThisEntity)
            where TEntity : class
        {
            var startingPoint = context.Model.FindEntityType(typeof(TEntity));
            if (startingPoint == null)
                throw new ArgumentException($"The class {typeof(TEntity).Name} is not in the database.", nameof(softDeleteThisEntity));

            var allEntities = context.Model
                .GetEntityTypes().ToList();

            void SetCascadeSoftDelete<T>(T principalInstance, IEntityType principal, int cascadeLevel)
            {
                var dependents = allEntities.SelectMany(x => x.GetForeignKeys()
                    .Where(y => y.PrincipalEntityType == principal)).Distinct().ToList();

                foreach (var foreignKey in dependents)
                {
                    var navProperty = foreignKey.PrincipalToDependent.PropertyInfo;
                    var principalNavs = context.Entry(principalInstance)
                        .Metadata.GetNavigations()
                        .ToList();
                }
            }

            SetCascadeSoftDelete(softDeleteThisEntity, startingPoint, 0);


        }


    }
}