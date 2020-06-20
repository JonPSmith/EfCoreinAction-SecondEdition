// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.SoftDeleteServices.Concrete
{
    public class CascadeSoftDelService
    {
        private readonly DbContext _context;
        private readonly bool _readEveryTime;

        private bool _set;
        private HashSet<object> _stopCircularLook;
        private int _numChanged;

        /// <summary>
        /// This provides a equivalent to a SQL cascade delete, but using a soft delete approach.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="readEveryTime">default is to read the relationships every time.
        /// But taking advantage of collections not loaded being null you can set this to false and save loading already loaded collections</param>
        public CascadeSoftDelService(DbContext context, bool readEveryTime = true)
        {
            _context = context;
            _readEveryTime = readEveryTime;
        }

        public int SetCascadeSoftDelete<TEntity>(TEntity softDeleteThisEntity)
            where TEntity : class
        {
            _stopCircularLook = new HashSet<object>();
            _numChanged = 0;
            _set = true;
            SetCascadeSoftDelete(softDeleteThisEntity, 1);
            _context.SaveChanges();
            return _numChanged;
        }

        public int ResetCascadeSoftDelete<TEntity>(TEntity softDeleteThisEntity)
            where TEntity : class
        {
            _stopCircularLook = new HashSet<object>();
            _numChanged = 0;
            _set = false;
            SetCascadeSoftDelete(softDeleteThisEntity, 1);
            _context.SaveChanges();
            return _numChanged;
        }

        private void SetCascadeSoftDelete(object principalInstance, byte cascadeLevel)
        {
            if (!(principalInstance is ICascadeSoftDelete castToCascadeSoftDelete && principalInstance.GetType().IsClass) || _stopCircularLook.Contains(principalInstance))
                return; //isn't something we need to consider, or we saw it before, so it returns 

            _stopCircularLook.Add(principalInstance);  //we keep a reference to this to stop the method going in a circular loop

            if (TestSetReset(castToCascadeSoftDelete, cascadeLevel))
                //If the entity shouldn't be changed then we leave this and any of it children
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
                        _context.Entry(principalInstance).Collection(navigation.PropertyInfo.Name).Load();
                        navValue = navigation.PropertyInfo.GetValue(principalInstance);
                    }
                    if (navValue == null)
                        return; //no relationship
                    foreach (var entity in navValue as IEnumerable)
                    {
                        SetCascadeSoftDelete(entity, (byte)(cascadeLevel + 1));
                    }
                }
                else
                {
                    if (_readEveryTime || navValue == null)
                    {
                        _context.Entry(principalInstance).Reference(navigation.PropertyInfo.Name).Load();
                        _context.Entry(principalInstance).Collection(navigation.PropertyInfo.Name).Load();
                        navValue = navigation.PropertyInfo.GetValue(principalInstance);
                    }
                    if (navValue == null)
                        return; //no relationship
                    SetCascadeSoftDelete(navValue, (byte)(cascadeLevel + 1));
                }
            }
        }

        private bool TestSetReset(ICascadeSoftDelete castToCascadeSoftDelete, byte cascadeLevel)
        {
            if (_set ? 
                    //_set = true   If the entity has already been soft deleted , then we don't change it, nor its child relationships
                    castToCascadeSoftDelete.SoftDeleteLevel != 0 
                    //_set = false  Don't reset if it was soft deleted at a lower level
                    : castToCascadeSoftDelete.SoftDeleteLevel < cascadeLevel)
                return true; //we don't change it and we exit

            //Else, yes we should change it 
            castToCascadeSoftDelete.SoftDeleteLevel = _set
                ? cascadeLevel
                : (byte)0;
            _numChanged++;

            return false;
        }



    }
}