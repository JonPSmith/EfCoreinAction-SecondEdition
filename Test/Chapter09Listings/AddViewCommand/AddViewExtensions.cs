// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Test.Chapter09Listings.AddViewCommand
{
    public static class AddViewExtensions                         //#A
    {
        public static void AddViewViaSql<TView>(                  //#B
            this MigrationBuilder migrationBuilder,               //#C
            string viewName,                                      //#D
            string tableName,                                     //#D
            string whereSql)                                      //#E
            where TView : class                                   //#F
        {
            if (!migrationBuilder.IsSqlServer())                  //#G
                throw new NotImplementedException("This command only works for SQL Server");

            var selectNamesString = string.Join(", ",             //#H
                typeof(TView).GetProperties()                     //#H
                .Select(x => x.Name));                            //#H

            var viewSql = 
                $"CREATE OR ALTER VIEW {viewName} AS " +          //#I
                $"SELECT {selectNamesString} FROM {tableName} " + //#I
                $"WHERE {whereSql}";                              //#I

            migrationBuilder.Sql(viewSql);                        //#J
        }
    }
    /*******************************************************************
    #A An extension method must be in a static class
    #B The method needs the class that is mapped to the view so that it can get its properties
    #C It has the MigrationBuilder so that it can access its methods, in this case Sql method
    #D The method needs the name to use for the View and the name of the table it is selecting from
    #E Views have a Where clause that filters the results returned by the view
    #F This ensures the TView type is a class
    #G This method throws an exception if the database isn't SQL Server because it uses a SQL Server View format
    #H This gets the names of the properties in the class mapped to the view and uses them as column names
    #I This creates the SQL command to create/update a View
    #J Finally it uses MigrationBuilder's Sql method to add it to the database
     *******************************************************************/
}