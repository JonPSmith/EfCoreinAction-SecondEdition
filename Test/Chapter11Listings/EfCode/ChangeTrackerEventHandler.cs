// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Test.Chapter11Listings.EfCode
{
    public class ChangeTrackerEventHandler      //#A                          
    {
        private readonly ILogger _logger;

        public ChangeTrackerEventHandler(DbContext context, 
            ILogger logger)
        {
            _logger = logger;                                        //#B

            context.ChangeTracker.Tracked += TrackedHandler;         //#C
            context.ChangeTracker.StateChanged += StateChangeHandler;//#D
        }

        private void TrackedHandler(object sender,                       //#E
            EntityTrackedEventArgs args)                                 //#E
        {                                                                
            if (args.FromQuery)                                          //#F
                return;                                                  //#F

            var message = $"Entity: {SimpleEntity(args.Entry)}. " +      //#G
                $"Was {args.Entry.State}";                               //#G
            _logger.LogInformation(message);                             //#G
        }                                                                //#G

        private void StateChangeHandler(object sender,                   //#H
            EntityStateChangedEventArgs args)                            //#H
        {                                                                //#H
            var message = $"Entity: {SimpleEntity(args.Entry)}. " +      //#H
                $"Was {args.OldState} and went to {args.NewState}";      //#H
            _logger.LogInformation(message);                             //#H
        }                                                                //#H

        private string SimpleEntity(EntityEntry entry)
        {
            return entry.DebugView.ShortView.Substring(0, entry.DebugView.ShortView.IndexOf('}') + 1);
        }
    }
    /******************************************************
    #A This class is used in your DbContext to log changes
    #B You will log to ILogger
    #C This adds a Tracked event handler
    #D This adds a StateChanged event handler
    #E This handles Tracked events 
    #F We do not want to log entities that are read in
    #G This forms a useful message on Add or Attach
    #H The StateChanged event handler logs any changes 
     *******************************************************/
}