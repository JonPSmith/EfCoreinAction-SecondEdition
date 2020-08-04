// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Test.Chapter11Listings.EfClasses;
using Test.Chapter11Listings.Interfaces;

namespace Test.Chapter11Listings.EfCode
{
    public class Chapter11DbContext : DbContext                 //#A
    {
        private ChangeTrackerEventHandler _trackerEventHandler; //#B

        public Chapter11DbContext(
            DbContextOptions<Chapter11DbContext> options,
            ILogger logger = null)                              //#C
            : base(options)
        {
            if (logger != null)                                 //#D
                _trackerEventHandler = new                      //#E
                    ChangeTrackerEventHandler(this, logger);    //#E
        }
        /**************************************************************
        #A This is your application DbContext you want to log changes from
        #B You need an instance of the event handler class while the DbContext exists
        #C You add a ILogger to the constructor.
        #D If an ILogger is available then you register the handlers
        #E This creates the event handler class, which registers the event handlers
         ************************************************************/

        public DbSet<MyEntity> MyEntities { get; set; }
        public DbSet<OneEntityOptional> OneOptionalEntities { get; set; }
        public DbSet<OneEntityRequired> OneEntityRequired { get; set; }
        public DbSet<ManyEntity> ManyEntities { get; set; }

        public DbSet<NotifyEntity> Notify { get; set; }
        public DbSet<Notify2Entity> Notify2 { get; set; }

        public DbSet<EntityAddUpdate> LoggedEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotifyEntity>()
                .HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangedNotifications);
            modelBuilder.Entity<Notify2Entity>()
                .HasChangeTrackingStrategy(ChangeTrackingStrategy.ChangingAndChangedNotifications);
        }

        private void AddUpdateChecks()                                  //#A
        {
            ChangeTracker.DetectChanges();                              //#B
            foreach (var entity in ChangeTracker.Entries()              //#C
                .Where(e =>                                             //#C
                    e.State == EntityState.Added ||                     //#C
                    e.State == EntityState.Modified))                   //#C
            {
                var tracked = entity.Entity as ICreatedUpdated;         //#D
                tracked?.LogChange(entity);                                //#E
            }
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess) //#F
        {
            AddUpdateChecks();                                          //#G
            try
            {
                ChangeTracker.AutoDetectChangesEnabled = false;         //#H
                return base.SaveChanges(acceptAllChangesOnSuccess);     //#I
            }
            finally
            {
                ChangeTracker.AutoDetectChangesEnabled = true;          //#J
            }
        }
        /********************************************************
        #A This private method will be called from SaveChanges and SaveChangesAsync
        #B It calls DetectChanges to make sure all the updates have been found
        #C It loops through all the tracked entities that have a State of Added or Modified
        #D If the Added/Modified entity has the ICreatedUpdated, then the tracked isn't null
        #E So we call the LogChange command. In this example we don't have the UserId available
        #F You override SaveChanges (and SaveChangesAsync - not shown)
        #G You call the AddUpdateChecks, which contains a call to ChangeTracker.DetectChanges()
        #H Because DetectChanges has been call we tell SaveChanges to call it again (for performance reasons)
        #I You call the base.SaveChanges that you overrided
        #J Finally to turn the AutoDetectChangesEnabled back on
         *******************************************************/

        public override Task<int> SaveChangesAsync(                    
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            AddUpdateChecks();
            try
            {
                ChangeTracker.AutoDetectChangesEnabled = false;
                return base.SaveChangesAsync(
                    acceptAllChangesOnSuccess, cancellationToken);
            }
            finally
            {
                ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }


    }
}