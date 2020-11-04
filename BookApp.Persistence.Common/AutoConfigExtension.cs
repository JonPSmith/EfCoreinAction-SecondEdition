// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BookApp.Domain.Books.SupportTypes;
using BookApp.Domain.Orders.SupportTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BookApp.Persistence.Common
{
    public static class AutoConfigExtension
    {
        public static void AutoConfigureTypes(this ModelBuilder modelBuilder)
        {
            var utcConverter = new ValueConverter<DateTime, DateTime>(
                toDb => toDb,
                fromDb =>
                    DateTime.SpecifyKind(fromDb, DateTimeKind.Utc));


            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var entityProperty in entityType.GetProperties())
                {
                    if (entityProperty.ClrType == typeof(DateTime)
                        && entityProperty.Name.EndsWith("Utc"))
                    {
                        entityProperty.SetValueConverter(utcConverter);
                    }

                    if (entityProperty.ClrType == typeof(DateTime)
                        && entityProperty.Name == "LastUpdatedUtc")
                    {
                        entityProperty.IsIndex();
                    }

                    if (entityProperty.ClrType == typeof(decimal)
                        && entityProperty.Name.Contains("Price"))
                    {
                        entityProperty.SetPrecision(9);
                        entityProperty.SetScale(2);
                    }

                    if (entityProperty.ClrType == typeof(string)
                        && entityProperty.Name.EndsWith("Url"))
                    {
                        entityProperty.SetIsUnicode(false);
                    }
                }
            }
        }

        public static void AutoConfigureQueryFilters<TContext>(this ModelBuilder modelBuilder, DbContext currentContext)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    entityType.AddSoftDeleteQueryFilter(
                        MyQueryFilterTypes.SoftDelete);
                }

                if (typeof(IUserId).IsAssignableFrom(entityType.ClrType)
                    && currentContext is IUserId userId )
                {
                    entityType.AddSoftDeleteQueryFilter(
                        MyQueryFilterTypes.UserId, userId);
                }
            }
        }
    }
}