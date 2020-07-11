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
    public enum MyQueryFilterTypes { SoftDelete, UserId }          //#A

    public class QueryFilterAutoConfig                             //#B
    {
        private readonly IUserId _userIdProvider;                             //#C

        public QueryFilterAutoConfig(IUserId userIdProvider)                  //#C
        {                                                          //#C
            _userIdProvider = userIdProvider;                                      //#C
        }

        public void SetQueryFilter(IMutableEntityType entityData,  //#D
            MyQueryFilterTypes queryFilterType)                    //#D
        {
            var methodName = $"Get{queryFilterType}Filter";        //#E
            var methodToCall = this.GetType().GetMethod(methodName,//#E
                    BindingFlags.NonPublic | BindingFlags.Instance)//#E
                .MakeGenericMethod(entityData.ClrType);            //#E
            var filter = methodToCall                              //#F
                .Invoke(this, new object[] { });                   //#F
            entityData.SetQueryFilter((LambdaExpression)filter);   //#G
        }

        private LambdaExpression GetUserIdFilter<TEntity>()//#H
            where TEntity : class, IUserId                 //#H
        {                                                  //#H
            Expression<Func<TEntity, bool>> filter =       //#H
                x => x.UserId == _userIdProvider.UserId;                  //#H
            return filter;                                 //#H
        }                                                  //#H

        private LambdaExpression GetSoftDeleteFilter<TEntity>()//#I
            where TEntity : class, ISoftDelete                 //#I
        {                                                      //#I
            Expression<Func<TEntity, bool>> filter =           //#I
                x => !x.SoftDeleted;                           //#I
            return filter;                                     //#I
        }                                                      //#I
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