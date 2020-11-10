// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace BookApp.UI.Models
{
    public enum BookAppMenuSettings { Basic, Chapter15, Chapter16, All}
    public class BookAppSettings
    {
        public bool CosmosAvailable { get; set; }
        public BookAppMenuSettings MenuSet { get; set; }
        public string DbNameSuffix { get; set; }
        public bool ProductionDbs { get; set; }
    }
}