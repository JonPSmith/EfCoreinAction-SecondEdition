// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Test.Chapter09Listings.AddViewCommand
{
    public static class AddViewExtensions
    {
        public static MigrationBuilder AddViewViaDirectSql<TEntity, TView>(
            this MigrationBuilder migrationBuilder,
            string viewName,
            string tableName,
            string whereSql)
            where TView : class 
        {
            if (!migrationBuilder.IsSqlServer())
                throw new NotImplementedException("This command only works for SQL Server");

            var selectNamesString = string.Join(", ", 
                typeof(TView).GetProperties()
                .Select(x => x.Name));

            var viewSql = $"CREATE OR REPLACE VIEW [{viewName}] AS " + 
                $"SELECT {selectNamesString} FROM {tableName} WHERE {whereSql}";
            migrationBuilder.Sql(viewSql);

            return migrationBuilder;
        }
    }
}