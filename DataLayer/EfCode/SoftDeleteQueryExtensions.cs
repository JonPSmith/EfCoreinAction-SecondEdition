// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataLayer.EfCode
{
    public enum MyQueryFilterTypes { SoftDelete, UserId }       //#A

    public static class SoftDeleteQueryExtensions              //#B
    {
        public static void AddSoftDeleteQueryFilter(            //#C
            this IMutableEntityType entityData,                 //#D
            MyQueryFilterTypes queryFilterType,                 //#E
            IUserId userIdProvider = null)                      //#F
        {
            var methodName = $"Get{queryFilterType}Filter";       //#G
            var methodToCall = typeof(SoftDeleteQueryExtensions)  //#G
                .GetMethod(methodName,                            //#G
                    BindingFlags.NonPublic | BindingFlags.Static) //#G
                .MakeGenericMethod(entityData.ClrType);           //#G
            var filter = methodToCall                             //#G
                .Invoke(null, new object[] { userIdProvider });   //#G
            entityData.SetQueryFilter((LambdaExpression)filter);  //#H 
            
            if (queryFilterType == MyQueryFilterTypes.SoftDelete) //#I
                entityData.AddIndex(entityData.FindProperty(      //#I
                    nameof(ISoftDelete.SoftDeleted)));            //#I
            if (queryFilterType == MyQueryFilterTypes.UserId)     //#J
                entityData.AddIndex(entityData.FindProperty(      //#J
                    nameof(IUserId.UserId)));                     //#J
        }

        private static LambdaExpression GetUserIdFilter<TEntity>(     //#K
            IUserId userIdProvider)                                   //#K
            where TEntity : class, IUserId                            //#K
        {                                                             //#K
            Expression<Func<TEntity, bool>> filter =                  //#K
                x => x.UserId == userIdProvider.UserId;               //#K
            return filter;                                            //#K
        }                                                             //#K

        private static LambdaExpression GetSoftDeleteFilter<TEntity>( //#L
            IUserId userIdProvider)                                   //#L
            where TEntity : class, ISoftDelete                        //#L
        {                                                             //#L
            Expression<Func<TEntity, bool>> filter =                  //#L
                x => !x.SoftDeleted;                                  //#L
            return filter;                                            //#L
        }
    }
    /*******************************************************
    #A Defines the different type of LINQ query to put in the Query Filter 
    #B This is an static extension class 
    #C Call this method to set up the query filter
    #D First parameter comes from EF Core and allows you to add a query filter
    #E Second parameter allows you to pick which type of query filter to add
    #F Third, optional property holds a copy of the current DbContext instance so that the UserId will be the current one
    #G Create the correctly typed method to create the Where LINQ expression to use in the Query Filter
    #H Uses the filter returns by the created type method in the SetQueryFilter method 
    #I Add an index on the SoftDeleted property for better performance
    #J Add an index on the UserId property for better performance
    #K Creates a query that is only true if the _userId matches the UserID in the entity class
    #L Creates a query that is only true if the SoftDeleted property is false
     ********************************************************/

}