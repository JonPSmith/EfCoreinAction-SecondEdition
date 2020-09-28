// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit.Abstractions;

namespace Test.TestHelpers
{
    public static class DatabaseMetadata
    {
        public static string GetTableName<TEntity>(this DbContext context)
        {
            var efType = context.Model.FindEntityType(typeof(TEntity));
            return efType.GetTableName();
        }

        public static IEnumerable<IProperty> GetProperties<TEntity>(this DbContext context)
        {
            var efType = context.Model.FindEntityType(typeof(TEntity));
            return efType.GetProperties();
        }

        public static string GetColumnName<TEntity, TProperty>(this DbContext context, TEntity source, 
            Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity));
            var propInfo = GetPropertyInfoFromLambda(model);
            return efType.FindProperty(propInfo.Name).GetColumnName();
        }

        public static string GetColumnName<TEntity>(this DbContext context, string propertyName) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity));
            return efType.FindProperty(propertyName).GetColumnName();
        }

        public static string GetColumnStoreType<TEntity, TProperty>(this DbContext context, 
            TEntity source, Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity));
            var propInfo = GetPropertyInfoFromLambda(model);
            return efType.FindProperty(propInfo.Name).GetColumnType();
        }

        public static string GetColumnStoreType<TEntity>(this DbContext context, string propertyName) where TEntity : class
        {
            var efType = context.Model.FindEntityType(typeof(TEntity));
            return efType.FindProperty(propertyName).GetColumnType();
        }

        public static void ListPropertiesAndForeignKeys<TEntity>(this DbContext context, ITestOutputHelper output)
            where TEntity : class
        {
            var modelProps = context.Model.FindEntityType(typeof(TEntity)).GetProperties();
            var fks = context.Model.FindEntityType(typeof(TEntity)).GetForeignKeys();

            //VERIFY
            output.WriteLine($"Properties  for {typeof(TEntity).Name}");
            foreach (var modelProp in modelProps)
            {
                output.WriteLine("   " + modelProp.ToString());
            }
            output.WriteLine("Foreign keys");
            foreach (var fk in fks)
            {
                output.WriteLine("   " + fk.ToString());
            }
        }

        //---------------------------------------------------
        //private methods

        public static PropertyInfo GetPropertyInfoFromLambda<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> model) where TEntity : class
        {
            var memberEx = (MemberExpression)model.Body;
            if (memberEx == null)
                throw new ArgumentNullException(nameof(model), "You must supply a LINQ expression that is a property.");

            var propInfo = typeof(TEntity).GetProperty(memberEx.Member.Name);
            if (propInfo == null)
                throw new ArgumentNullException(nameof(model), "The member you gave is not a property.");
            return propInfo;
        }
    }
}