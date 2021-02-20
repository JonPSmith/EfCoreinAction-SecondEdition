// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using System.Reflection;
using BookApp.Domain.Books.SupportTypes;
using BookApp.Domain.Orders.SupportTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookApp.Persistence.Common
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
            if (queryFilterType == MyQueryFilterTypes.SoftDelete)
                entityData.AddIndex(entityData.FindProperty(nameof(ISoftDelete.SoftDeleted)));
            if (queryFilterType == MyQueryFilterTypes.UserId)
                entityData.AddIndex(entityData.FindProperty(nameof(IUserId.UserId)));
        }

        private static LambdaExpression GetUserIdFilter<TEntity>(     //#I
            IUserId userIdProvider)                                   //#I
            where TEntity : class, IUserId                            //#I
        {                                                             //#I
            Expression<Func<TEntity, bool>> filter =                  //#I
                x => x.UserId == userIdProvider.UserId;               //#I
            return filter;                                            //#I
        }                                                             //#I

        private static LambdaExpression GetSoftDeleteFilter<TEntity>( //#J
            IUserId userIdProvider)                                   //#J
            where TEntity : class, ISoftDelete                        //#J
        {                                                             //#J
            Expression<Func<TEntity, bool>> filter =                  //#J
                x => !x.SoftDeleted;                                  //#J
            return filter;                                            //#J
        }
    }
    /*******************************************************
    #A Defines the different type of LINQ query to put in the Query Filter 
    #B This is an static extension class 
    #C Call this method to set up the query filter
    #D First parameter comes from EF Core and allows you to add a query filter
    #E Second parameter allows you to pick which type of query filter to add
    #F Third, optional property holds a copy of the current DbContext instance so that the UserId will be the current one
    #G Create the correctly typed method to create the where LINQ expression to use in the Query Filter
    #H Uses the filter returns by the created type method in the SetQueryFilter method 
    #I Creates a query that is only true if the _userId matches the UserID in the entity class
    #J Creates a query that is only true if the SoftDeleted property is false
     ********************************************************/

}