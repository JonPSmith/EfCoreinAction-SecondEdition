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
        private readonly bool _nullNavigationalMeansNotLoaded;
        private HashSet<object> _stopCircularLook;
        private int _numChanged;

        public CascadeSoftDelService(DbContext context, bool nullNavigationalMeansNotLoaded = false)
        {
            _context = context;
            _nullNavigationalMeansNotLoaded = nullNavigationalMeansNotLoaded;
        }

        public int CascadeSoftDelete<TEntity>(TEntity softDeleteThisEntity)
            where TEntity : class
        {
            _stopCircularLook = new HashSet<object>();
            _numChanged = 0;
            SetCascadeSoftDelete(softDeleteThisEntity, 1);
            _context.SaveChanges();
            return _numChanged;
        }

        private void SetCascadeSoftDelete(object principalInstance, byte cascadeLevel)
        {
            if (!(principalInstance is ICascadeSoftDelete castToCascadeSoftDelete && principalInstance.GetType().IsClass) || _stopCircularLook.Contains(principalInstance))
                return; //isn't something we need to consider, or we saw it before, so it returns 

            _stopCircularLook.Add(principalInstance);  //we keep a reference to this to stop the method going in a circular loop

            if (castToCascadeSoftDelete.SoftDeleteLevel != 0)
                //If the entity has already been soft deleted, then we don't change it, nor its child relationships
                return;
                
            castToCascadeSoftDelete.SoftDeleteLevel = cascadeLevel;
            _numChanged++;

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
                var navValue = navigation.PropertyInfo.GetValue(principalInstance);
                if (navigation.IsCollection)
                {
                    if (_nullNavigationalMeansNotLoaded && navValue == null)
                    {
                        //only load if null
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
                    if (_nullNavigationalMeansNotLoaded && navValue == null)
                    {
                        //only load if null
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


    }
}