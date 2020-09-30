// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter08Listings.PropertyBags
{
    public class PropertyBagsDbContext : DbContext
    {
        private readonly TableSpec _tableSpec; //#A

        public PropertyBagsDbContext(
            DbContextOptions<PropertyBagsDbContext> options, 
            TableSpec tableSpec)     //#A
            : base(options)
        {
            _tableSpec = tableSpec;  //#A
        }

        public DbSet<Dictionary<string, object>> MyTable        //#B
            => Set<Dictionary<string, object>>(_tableSpec.Name);//#B

        //!!!!!!!!!!!!!!!!!! Leave out of book - only for testing
        public DbSet<TestClass> TestAccess { get; set; }

        protected override void OnModelCreating
            (ModelBuilder modelBuilder)
        { 
            modelBuilder.SharedTypeEntity          //#C
                <Dictionary<string, object>>(      //#C
                    _tableSpec.Name, b =>     //#D
            {
                foreach (var prop in _tableSpec.Properties) //#E
                {
                    var propConfig = b.IndexerProperty( //#F
                        prop.PropType, prop.Name);   //#F
                    if (prop.AddRequired)            //#G
                        propConfig.IsRequired();     //#G
                }
            }).Model.AddAnnotation("Table", _tableSpec.Name); //#H
            //!!!!!!!!!!!!!!!!!! Leave out of book - only for testing
            modelBuilder.Entity<TestClass>().ToView(_tableSpec.Name);
        }
    }
    /*****************************************************************
    #A You pass in a class containing the specification of the table and properties
    #B The DbSet called MyTable  links to the SharedType entity built in OnModelCreating
    #C This defines a SharedType entity type
    #D You give this shared entity type a name so that you can refer to it
    #E This adds each property in turn from the tableSpec
    #F This adds an index property - it finds the primary key based on its name
    #G This sets the property to not being null - only needed on nullable types like string 
    #H Now you map it to the table you want to access
     *********************************************************/

}