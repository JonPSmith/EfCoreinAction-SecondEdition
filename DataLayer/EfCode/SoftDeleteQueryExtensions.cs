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
    public enum MyQueryFilterTypes { SoftDelete, UserId }

    public static class SoftDeleteQueryExtensions
    {
        public static void AddSoftDeleteQueryFilter(this IMutableEntityType entityData,
            MyQueryFilterTypes queryFilterType, IUserId userIdProvider = null)
        {
            var methodName = $"Get{queryFilterType}Filter";
            var methodToCall = typeof(SoftDeleteQueryExtensions)
                .GetMethod(methodName,
                    BindingFlags.NonPublic | BindingFlags.Static)
                .MakeGenericMethod(entityData.ClrType);
            var filter = methodToCall.Invoke(null, new object[] { userIdProvider });
            entityData.SetQueryFilter((LambdaExpression)filter);
        }

        private static LambdaExpression GetUserIdFilter<TEntity>(
            IUserId userIdProvider)//#H
            where TEntity : class, IUserId                 //#H
        {                                                  //#H
            Expression<Func<TEntity, bool>> filter =       //#H
                x => x.UserId == userIdProvider.UserId;                  //#H
            return filter;                                 //#H
        }                                                  //#H

        private static LambdaExpression GetSoftDeleteFilter<TEntity>(
            IUserId userIdProvider)
            where TEntity : class, ISoftDelete
        {
            Expression<Func<TEntity, bool>> filter = x => !x.SoftDeleted;
            return filter;
        }
    }
    /*******************************************************
    #A This defines the different type of LINQ query to put in the Query Filter 
    #B This class with by used in the Book App's DbContext
    #C It is created every time a new DbContext is created, which means the userId is current user's ID
    #D This method is called if the configuration code finds an Interface that is used for Query Filter
    #E This code finds a generic method and creates it with the correct entity type
    #F Invoking this method returns the correct filter for that entity class
    #G You use the SetQueryFilter with that filter
    #H This creates a query that is only true if the _userId matches the UserID in the entity class
    #I This creates a query that is only true if the SoftDeleted property is false
     ********************************************************/

}