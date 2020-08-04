// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Test.Chapter11Listings.Interfaces;

namespace Test.Chapter11Listings.EfClasses
{
    public class CreatedUpdatedInfo : ICreatedUpdated //#A
    {
        public DateTime WhenCreatedUtc { get; private set; }//#B
        public Guid CreatedBy { get; private set; }         //#B
        public DateTime LastUpdatedUtc { get; private set; }//#B
        public Guid LastUpdatedBy { get; private set; }     //#B
        public void LogChange(EntityEntry entry, Guid userId = default) //#C
        {
            if (entry.State != EntityState.Added &&         //#D
                entry.State != EntityState.Modified)        //#D
                return;                                     //#D

            var timeNow = DateTime.UtcNow;                  //#E
            LastUpdatedUtc = timeNow;                       //#F
            LastUpdatedBy = userId;                         //#F
            if (entry.State == EntityState.Added)           //#G
            {
                WhenCreatedUtc = timeNow;                   //#G
                CreatedBy = userId;                         //#G
            }
            else
            {
                entry.Property(                              //#H
                    nameof(ICreatedUpdated.LastUpdatedUtc))  //#H
                    .IsModified = true;                      //#H
                entry.Property(                              //#H
                    nameof(ICreatedUpdated.LastUpdatedBy))   //#H
                    .IsModified = true;                      //#H
            }
        }
    }
    /**********************************************************
    #A Entity class inherits ICreatedUpdated, which means any addition/update of the entity is logged.
    #B These properties have private setters so that only the LogChange method can change them.
    #C Its job is to update the created and updated properties. It is passed the UserId if available
    #D This method only handles Added or Modified States
    #E Obtains the current time so that an add and update time will be the same on create
    #F It always sets the LastUpdatedUtc and LastUpdatedBy
    #G If its an add, then you update the WhenCreatedUtc and the CreatedBy properties
    #H For performance reasons you turned off DetectChanges, so you must manually mark the properties as modified
     *************************************************************/
}