// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using BookApp.Infrastructure.AppParts;

namespace BookApp.UI.Services
{
    public class MenuBuilder : IMenuBuilder
    {
        private readonly BookAppSettings _settings;

        private static readonly Dictionary<BookAppMenuSettings, List<MenuData>> PossibleMenus 
            = new Dictionary<BookAppMenuSettings, List<MenuData>>
        {
            [BookAppMenuSettings.Basic] = new List<MenuData>
            {
                new MenuData("DefaultSql", "Books"),
                new MenuData("Orders", "Your Orders")
            },
            [BookAppMenuSettings.SqlOnly] = new List<MenuData>
            {
                new MenuData("DefaultSql", "SQL (LINQ)"),
                new MenuData("UdfsSql", "SQL (+UDFs)"),
                new MenuData("DapperSql", "SQL (Dapper)"),
                new MenuData("CachedSql", "SQL (+cache)")
            },
            [BookAppMenuSettings.SqlAndCosmos] = new List<MenuData>
            {
                new MenuData("CosmosEf", "Cosmos (EF)"),
                new MenuData("CosmosDirect", "Cosmos (Direct)"),
                new MenuData("CachedNoCountSql", "SQL (+cacheNC)"),
                new MenuData("DapperNoCountSql", "SQL (DapperNC)")
            },
            [BookAppMenuSettings.CosmosOnly] = new List<MenuData>
            {
                new MenuData("CosmosEf", "Cosmos (EF)"),
                new MenuData("CosmosDirect", "Cosmos (Direct)"),
                new MenuData("Orders", "Your Orders")
            }
        };

        public MenuBuilder(BookAppSettings settings)
        {
            _settings = settings;
        }

        public List<MenuData> GetMenus()
        {
            var menuSet = _settings.MenuSet == BookAppMenuSettings.SqlAndCosmos && !_settings.CosmosAvailable
                ? BookAppMenuSettings.SqlOnly
                : _settings.MenuSet;


            return PossibleMenus[menuSet];
        }
    }
}