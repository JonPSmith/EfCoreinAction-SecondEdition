// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericEventRunner.DomainParts;

namespace BookApp.Domain.Books.DomainEvents
{
    public enum BookChangeTypes { Added, Updated, Deleted } //#A

    [RemoveDuplicateEvents] //#B
    public class BookChangedEvent : IEntityEvent //#C
    {
        public BookChangedEvent(BookChangeTypes bookChangeType)  //#D
        {                                                        //#D
            BookChangeType = bookChangeType;                     //#D
        }                                                        //#D

        public BookChangeTypes BookChangeType { get; }  //#E
    }
    /*****************************************************************
    #A these are the three types of changes that need mapping to the Cosmos DB database
    #B This attribute causes the GenericEventRunner to remove other events that are for the same Book instance
    #C When an event is created you must say what type of change the Book has gone through
    #D This is used by the event handler to work out whether to add, update or delete the CosmosBook
    #E This holds the type of change for the event handler to use
     ****************************************************************/
}