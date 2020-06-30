// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.SoftDeleteServices.Concrete.Internal
{
    internal static class LoadEntityExtensions
    {
        public static TEntity LoadEntityViaPrimaryKeys<TEntity>(this DbContext context, bool withIgnoreFilter, params object[] keyValues)
            where TEntity : class
        {
            var entityType = context.Model.FindEntityType(typeof(TEntity));
            if (entityType == null)
                throw new ArgumentException($"The class {typeof(TEntity).Name} was not found in the {context.GetType().Name} DbContext.");
            if (entityType.IsOwned())
                throw new ArgumentException($"The class {typeof(TEntity).Name} is an Owned class and can't be loaded on its own.");
            if (entityType.FindPrimaryKey() == null)
                throw new ArgumentException($"The class {typeof(TEntity).Name} is read-only.");

            var keyProps = context.Model.FindEntityType(typeof(TEntity))
                .FindPrimaryKey().Properties.Select(x => x.PropertyInfo).ToList();
            if(keyProps.Any(x => x == null))
                throw new NotImplementedException("This library cannot handle primary keys in shadow properties or backing fields");

            var query = withIgnoreFilter
                ? context.Set<TEntity>().IgnoreQueryFilters()
                : context.Set<TEntity>();
            return query.SingleOrDefault(CreateFilter<TEntity>(keyProps, keyValues));
        }

        private static Expression<Func<T, bool>> CreateFilter<T>(this IList<PropertyInfo> keyProperties, object[] keyValues)
        {


            if (keyProperties.Count != keyValues.Length)
                throw new ArgumentException("The number of keys values provided does not match the number of keys in the entity class.");

            var x = Expression.Parameter(typeof(T), "x");
            var filterParts = keyProperties.Select((t, i) => BuildEqual<T>(x, t, keyValues[i])).ToList();
            var combinedFilter = CombineFilters(filterParts);

            return Expression.Lambda<Func<T, bool>>(combinedFilter, x);
        }

        private static Expression CombineFilters(List<BinaryExpression> filterParts)
        {
            var result = filterParts.First();
            for (int i = 1; i < filterParts.Count; i++)
                result = Expression.AndAlso(result, filterParts[i]);

            return result;
        }

        private static BinaryExpression BuildEqual<T>(ParameterExpression p, PropertyInfo prop, object expectedValue)
        {
            var m = Expression.Property(p, prop);
            var c = Expression.Constant(expectedValue);
            var ex = Expression.Equal(m, c);
            return ex;
        }
    }
}