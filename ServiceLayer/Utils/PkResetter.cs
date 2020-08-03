// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.Utils
{
    public class PkResetter
    {
        private readonly DbContext _context;
        private readonly HashSet<object> _stopCircularLook;                //#A

        public PkResetter(DbContext context)
        {
            _context = context;
            _stopCircularLook = new HashSet<object>();
        }

        public void ResetPksEntityAndRelationships(object entityToReset)   //#B
        {
            if (_stopCircularLook.Contains(entityToReset))                 //#C
                return;                                                    //#C

            _stopCircularLook.Add(entityToReset);                          //#D

            var entry = _context.Entry(entityToReset);                     //#E
            if (entry == null)                                             //#E
                return;                                                    //#E

            var primaryKey = entry.Metadata.FindPrimaryKey();              //#F
            if (primaryKey != null)                                        //#G
            {                                                              //#G
                foreach (var primaryKeyProperty in primaryKey.Properties)  //#G
                {                                                          //#G
                    primaryKeyProperty.PropertyInfo                        //#G
                        .SetValue(entityToReset,                           //#G
                        GetDefaultValue(                                   //#G
                            primaryKeyProperty.PropertyInfo.PropertyType));//#G
                }                                                          //#G
            }                                                              //#G

            foreach (var navigation in entry.Metadata.GetNavigations())    //#H
            {
                var navProp = navigation.PropertyInfo;                     //#I

                var navValue = navProp.GetValue(entityToReset);            //#J
                if (navValue == null)                                      //#K
                    continue;                                              //#K

                if (navigation.IsCollection)                               //#L
                {
                    foreach (var item in (IEnumerable)navValue)            //#M
                    {                                                      //#M
                        ResetPksEntityAndRelationships(item);              //#M
                    }                                                      //#M
                }
                else
                {
                    ResetPksEntityAndRelationships(navValue);              //#N
                }
            }
        }
        /*********************************************************************
        #A This is used to stop circular recursive steps
        #B This method will recursively look at all the linked entities and reset their primary key
        #C If the method has already looked at this entity it returns
        #D This remembers that this entity has been visited by this method
        #E This deals with an entity that isn't known by your configuration
        #F This gets the primary key information for this entity
        #G This resets every property used in the primary key to its default value
        #H This gets all the navigational properties for this entity
        #I This gets property that contains the navigation property
        #J Then get the navigation property value
        #K If null then skip navigation property
        #L If the navigation property is collection, then it visits every entity
        #M This recursively visits each entity in the collection
        #N If it's a singleton, then it recursively visits that entity
         */

        // Thanks to StackOverflow https://stackoverflow.com/a/2490274/1434764
        private static object GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);

            return null;
        }
    }
}