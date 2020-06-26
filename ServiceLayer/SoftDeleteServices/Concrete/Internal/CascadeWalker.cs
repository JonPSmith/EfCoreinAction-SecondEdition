// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ServiceLayer.SoftDeleteServices.Concrete.Internal
{
    internal class CascadeWalker
    {
        private readonly DbContext _context;
        private readonly bool _set;
        private readonly bool _readEveryTime;

        private readonly HashSet<object> _stopCircularLook = new HashSet<object>();
        public int NumChanged { get; private set; } = 0;

        public CascadeWalker(DbContext context, bool set, bool readEveryTime)
        {
            _context = context;
            _set = set;
            _readEveryTime = readEveryTime;
        }

        public void AlterCascadeSoftDelete(object principalInstance, byte cascadeLevel)
        {
            if (!(principalInstance is ICascadeSoftDelete castToCascadeSoftDelete && principalInstance.GetType().IsClass) || _stopCircularLook.Contains(principalInstance))
                return; //isn't something we need to consider, or we saw it before, so it returns 

            _stopCircularLook.Add(principalInstance);  //we keep a reference to this to stop the method going in a circular loop

            if (TestSetReset(castToCascadeSoftDelete, cascadeLevel))
                //If the entity shouldn't be changed then we leave this entity and any of it children
                return;

            var principalNavs = _context.Entry(principalInstance)
                .Metadata.GetNavigations()
                .Where(x => !x.IsOnDependent && //navigational link goes to dependent entity(s)
                                                //The developer has set a Cascade delete behaviour (two options) on this link
                            (x.ForeignKey.DeleteBehavior == DeleteBehavior.ClientCascade || x.ForeignKey.DeleteBehavior == DeleteBehavior.Cascade))
                .ToList();

            foreach (var navigation in principalNavs)
            {
                if (navigation.PropertyInfo == null)
                    throw new NotImplementedException("Currently only works with navigation links that are properties");

                //It loads the current navigational value so that we can limit the number of database selects if the data is already loaded
                var navValue = navigation.PropertyInfo.GetValue(principalInstance);
                if (navigation.IsCollection)
                {
                    if (_readEveryTime || navValue == null)
                    {
                        LoadNavigationCollection(principalInstance, navigation);
                        navValue = navigation.PropertyInfo.GetValue(principalInstance);
                    }
                    if (navValue == null)
                        return; //no relationship
                    foreach (var entity in navValue as IEnumerable)
                    {
                        AlterCascadeSoftDelete(entity, (byte)(cascadeLevel + 1));
                    }
                }
                else
                {
                    if (_readEveryTime || navValue == null)
                    {
                        LoadNavigationSingleton(principalInstance, navigation);
                        navValue = navigation.PropertyInfo.GetValue(principalInstance);
                    }
                    if (navValue == null)
                        return; //no relationship
                    AlterCascadeSoftDelete(navValue, (byte)(cascadeLevel + 1));
                }
            }
        }

        private bool TestSetReset(ICascadeSoftDelete castToCascadeSoftDelete, byte cascadeLevel)
        {
            if (_set ?
                    //_set = true   If the entity has already been soft deleted , then we don't change it, nor its child relationships
                    castToCascadeSoftDelete.SoftDeleteLevel != 0
                    //_set = false  Don't reset if it was soft deleted value donesn't match - this stops previously deleted sub-groups being updeleted
                    :  castToCascadeSoftDelete.SoftDeleteLevel != cascadeLevel)
                return true; //we don't change it and we exit

            //Else, yes we should change it 
            castToCascadeSoftDelete.SoftDeleteLevel = _set
                ? cascadeLevel
                : (byte)0;
            NumChanged++;

            return false;
        }

        private void LoadNavigationCollection(object principalInstance, INavigation navigation)
        {
            if (_set)
                //For setting we can simple load it, as we don't want to set anything that is already set
                _context.Entry(principalInstance).Collection(navigation.PropertyInfo.Name).Load();
            else
            {
                //for undoing a soft delete we need to load the collection with a IgnoreQueryFilters method
                var navValueType = navigation.PropertyInfo.PropertyType;
                var innerType = navValueType.GetGenericArguments().Single();
                var genericHelperType =
                    typeof(GenericCollectionLoader<>).MakeGenericType(innerType);

                Activator.CreateInstance(genericHelperType, _context, principalInstance, navigation.PropertyInfo);
            }
        }

        private  class GenericCollectionLoader<TEntity> where TEntity : class
        {
            public GenericCollectionLoader(DbContext context, object principalInstance, PropertyInfo propertyInfo)
            {
                var query = context.Entry(principalInstance).Collection(propertyInfo.Name).Query();
                var collection = query.Provider.CreateQuery<TEntity>(query.Expression).IgnoreQueryFilters().ToList();
                propertyInfo.SetValue(principalInstance, collection);
            }
        }

        private void LoadNavigationSingleton(object principalInstance, INavigation navigation)
        {
            if (_set)
                //For setting we can simple load it, as we don't want to set anything that is already set
                _context.Entry(principalInstance).Reference(navigation.PropertyInfo.Name).Load();
            else
            {
                //for undoing a soft delete we need to load the collection with a IgnoreQueryFilters method
                var navValueType = navigation.PropertyInfo.PropertyType;
                var genericHelperType =
                    typeof(GenericSingletonLoader<>).MakeGenericType(navValueType);

                Activator.CreateInstance(genericHelperType, _context, principalInstance, navigation.PropertyInfo);
            }
        }

        private class GenericSingletonLoader<TEntity> where TEntity : class
        {
            public GenericSingletonLoader(DbContext context, object principalInstance, PropertyInfo propertyInfo)
            {
                var query = context.Entry(principalInstance).Reference(propertyInfo.Name).Query();
                var singleton = query.Provider.CreateQuery<TEntity>(query.Expression).IgnoreQueryFilters().SingleOrDefault();
                propertyInfo.SetValue(principalInstance, singleton);
            }
        }

    }
}