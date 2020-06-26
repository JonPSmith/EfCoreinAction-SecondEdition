// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ServiceLayer.SoftDeleteServices.Concrete.Internal
{
    internal class CascadeWalker
    {
        
        private readonly DbContext _context;
        private readonly CascadeSoftDelWhatDoing _whatDoing;
        private readonly bool _readEveryTime;

        private readonly HashSet<object> _stopCircularLook = new HashSet<object>();

        public CascadeSoftDeleteInfo Info { get; private set; }

        public CascadeWalker(DbContext context, CascadeSoftDelWhatDoing whatDoing, bool readEveryTime)
        {
            _context = context;
            _whatDoing = whatDoing;
            _readEveryTime = readEveryTime;

            Info = new CascadeSoftDeleteInfo(_whatDoing);
        }

        public void WalkEntitiesSoftDelete(object principalInstance, byte cascadeLevel)
        {
            if (!(principalInstance is ICascadeSoftDelete castToCascadeSoftDelete && principalInstance.GetType().IsClass) || _stopCircularLook.Contains(principalInstance))
                return; //isn't something we need to consider, or we saw it before, so it returns 

            _stopCircularLook.Add(principalInstance);  //we keep a reference to this to stop the method going in a circular loop

            if (ApplyChangeIfAppropriate(castToCascadeSoftDelete, cascadeLevel))
                //If the entity shouldn't be changed then we leave this entity and any of it children
                return;

            var principalNavs = _context.Entry(principalInstance)
                .Metadata.GetNavigations()
                .Where(x => !x.IsOnDependent && //navigational link goes to dependent entity(s)
                                                //The developer has whatDoing a Cascade delete behaviour (two options) on this link
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
                        WalkEntitiesSoftDelete(entity, (byte)(cascadeLevel + 1));
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
                    WalkEntitiesSoftDelete(navValue, (byte)(cascadeLevel + 1));
                }
            }
        }

        /// <summary>
        /// This checks if something has to be done for this entity
        /// If it should not be changed it returns true, which says don't go any deeper from this entity
        /// If it should be changed then it does the change and returns false
        /// </summary>
        /// <param name="castToCascadeSoftDelete"></param>
        /// <param name="cascadeLevel"></param>
        /// <returns></returns>
        private bool ApplyChangeIfAppropriate(ICascadeSoftDelete castToCascadeSoftDelete, byte cascadeLevel)
        {
            switch (_whatDoing)
            {
                case CascadeSoftDelWhatDoing.SoftDelete:
                    if (castToCascadeSoftDelete.SoftDeleteLevel != 0)
                        //If the entity has already been soft deleted , then we don't change it, nor its child relationships
                        return true;
                    castToCascadeSoftDelete.SoftDeleteLevel = cascadeLevel;
                    break;
                case CascadeSoftDelWhatDoing.UnSoftDelete:
                    if (castToCascadeSoftDelete.SoftDeleteLevel != cascadeLevel)
                        //Don't reset if it was soft deleted value doesn't match -this stops previously deleted sub-groups being updeleted
                        return true;
                    castToCascadeSoftDelete.SoftDeleteLevel = (byte)0;
                    break;
                case CascadeSoftDelWhatDoing.CheckWhatWillDelete:
                    if (castToCascadeSoftDelete.SoftDeleteLevel == 0)
                        return true;
                    break;
                case CascadeSoftDelWhatDoing.HardDeleteSoftDeleted:
                    if (castToCascadeSoftDelete.SoftDeleteLevel == 0)
                        return true;
                    _context.Remove(castToCascadeSoftDelete);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Info.NumFound++;

            return false;
        }

        private void LoadNavigationCollection(object principalInstance, INavigation navigation)
        {
            if (_whatDoing == CascadeSoftDelWhatDoing.SoftDelete)
                //For setting we can simple load it, as we don't want to whatDoing anything that is already whatDoing
                _context.Entry(principalInstance).Collection(navigation.PropertyInfo.Name).Load();
            else
            {
                //for everything else we need to load the collection with a IgnoreQueryFilters method
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
            if (_whatDoing == CascadeSoftDelWhatDoing.SoftDelete)
                //For setting we can simple load it, as we don't want to whatDoing anything that is already whatDoing
                _context.Entry(principalInstance).Reference(navigation.PropertyInfo.Name).Load();
            else
            {
                //for everything else we need to load the singleton with a IgnoreQueryFilters method
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