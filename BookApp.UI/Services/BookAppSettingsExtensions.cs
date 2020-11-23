// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using BookApp.Infrastructure.AppParts;
using Microsoft.Extensions.Configuration;

namespace BookApp.UI.Services
{
    public static class BookAppSettingsExtensions
    {
        public static BookAppSettings GetBookAppSettings(this IConfiguration config)
        {
            var setupNumString = config["SetupVersion"];
            if (setupNumString == null || !int.TryParse(setupNumString, out var versionNum))
                throw new InvalidOperationException("There must be a 'SetupVersion' integer in the appsettings.json file");

            return GetBookAppSettings(config, versionNum);
        }

        public static BookAppSettings GetBookAppSettings(this IConfiguration config, int versionNum)
        {
            var settings = new BookAppSettings();
            config.GetSection($"Setup{versionNum}").Bind(settings);
            if (settings.Title == null)
                throw new InvalidOperationException($"Could not find 'Setup{versionNum}' section in appsettings.json file");

            return settings;
        }
    }
}