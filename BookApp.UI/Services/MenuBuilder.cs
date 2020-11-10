// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using BookApp.UI.Models;
using Microsoft.Extensions.Options;

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
            [BookAppMenuSettings.Chapter15] = new List<MenuData>
            {
                new MenuData("DefaultSql", "SQL (LINQ)"),
                new MenuData("UtfsSql", "SQL (+UTFs)"),
                new MenuData("DapperSql", "SQL (Dapper)"),
                new MenuData("CachedSql", "SQL (cached)")
            },
            [BookAppMenuSettings.Chapter16] = new List<MenuData>
            {
                new MenuData("CosmosEf", "Cosmos (EF)"),
                new MenuData("CosmosDirect", "Cosmos (Direct)"),
                new MenuData("CachedSql", "SQL (cached)"),
                new MenuData("Orders", "Your Orders")
            },
        };

        public MenuBuilder(IOptions<BookAppSettings> settings)
        {
            _settings = settings.Value;
        }

        public List<MenuData> GetMenus()
        {
            var menuSet = _settings.MenuSet == BookAppMenuSettings.Chapter16 && !_settings.CosmosAvailable
                ? BookAppMenuSettings.Chapter15
                : _settings.MenuSet;


            return PossibleMenus[menuSet];
        }
    }
}