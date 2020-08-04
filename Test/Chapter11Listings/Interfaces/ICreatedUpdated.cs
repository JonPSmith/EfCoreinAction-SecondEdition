// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Test.Chapter11Listings.Interfaces
{
    public interface ICreatedUpdated      //#A
    {
        DateTime WhenCreatedUtc { get; }  //#B
        Guid CreatedBy { get; }           //#C
        DateTime LastUpdatedUtc { get; }  //#D
        Guid LastUpdatedBy { get; }       //#E

        void LogChange(EntityEntry entry, Guid userId = default); //#F
    }
    /***********************************************************
    #A Added to any entity class when the entity is added or updated.
    #B Holds the datetime when the entity was first added to the database
    #C Holds the UserId who created the entity
    #D Holds the datetime when the entity was last updated
    #E Holds the UserId who last updated the entity
    #F Called when the entity's state is Added or Modified State. Its job is to update the properties based on the state. 
     **************************************************************/
}