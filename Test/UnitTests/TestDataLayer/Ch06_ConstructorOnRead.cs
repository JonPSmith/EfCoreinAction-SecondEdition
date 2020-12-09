// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_ConstructorOnRead
    {
        private readonly ITestOutputHelper _output;

        public Ch06_ConstructorOnRead(ITestOutputHelper output)
        {
            _output = output;
        }

        public class ReviewGood 
        {
            public int Id { get; private set; }           //#A
            public string VoterName { get; private set; } //#A
            public int NumStars { get; set; }

            public ReviewGood                             //#B
                (string voterName)                        //#C
            {
                VoterName = voterName;                    //#D
                NumStars = 2;                             //#E
            }
        }
        /**************************************************************
        #A You can set your properties to have a private setter - EF Core can still set them
        #B The constructor doesn't need parameters for all the properties in the class. Also, the constructor can be any type of accessibility, e.g. public, private, etc.
        #C EF Core will look for a parameter with the same type and a name that matches the property (with matching of Pascal/Camel case versions of the name)
        #D The assignment should not include any changing of the data, otherwise you won't get the exact data that was in the database 
        #E Any assignment to a property that doesn't have a parameter is fine - EF Core will set that property after the constructor to the data read back from the database
         **************************************************************/

        public class ReviewGood2
        {
            public int Id { get; private set; }           
            public string VoterName { get; private set; } 
            public int NumStars { get; set; }

            public ReviewGood2(string voterName, int numStars)
            {
                VoterName = voterName;
                NumStars = numStars;
            }

            public ReviewGood2(string voterName)   //EF Core uses this ctor for reads                     
            {
                VoterName = voterName;
            }
        }

        public class ReviewBad
        {
            public int Id { get; set; }
            public string VoterName { get; set; }
            public int NumStars { get; set; }

            public ReviewBad(string voterName)
            {
                VoterName = "Name: "+voterName;
                NumStars = 2;
            }
        }

        public class ReviewBadCtor1
        {
            public int Id { get; set; }
            public string VoterName { get; set; }
            public int NumStars { get; set; }

            public ReviewBadCtor1( //#A
                string voterName, 
                int starRating)  //#B
            {
                VoterName = voterName;
                NumStars = starRating;
            }
        }
        /***********************************************************
        #A This is the only constructor in this class
        #B This parameter's name doesn't match the name of any property in this class, so EF Core can't use it to create an instance of the class when it is reading in data
         ***********************************************************/

        public class ReviewBadCtor2
        {
            public int Id { get; set; }
            public string VoterName { get; set; }
            public int NumStars { get; set; }

            private ReviewBadCtor2() {}
            public ReviewBadCtor2(string voterName, int unknownParam)
            {
                VoterName = voterName;
                NumStars = unknownParam;
            }
        }

        public class CtorDbContext : DbContext
        {
            private readonly bool _configureReviewBadCtor;

            public CtorDbContext(DbContextOptions options, bool configureReviewBadCtor) : base(options)
            {
                _configureReviewBadCtor = configureReviewBadCtor;
            }

            public DbSet<ReviewGood> ReviewGoods { get; set; }
            public DbSet<ReviewGood2> Review2Goods { get; set; } 
            public DbSet<ReviewBad> ReviewBads { get; set; }
            public DbSet<ReviewBadCtor2> ReviewBadCtor2s { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                if (_configureReviewBadCtor)
                    modelBuilder.Entity<ReviewBadCtor1>();
            }
        }

        [Fact]
        public void ReadReviewsGood()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CtorDbContext>();
            using var context = new CtorDbContext(options, false);
            context.Database.EnsureCreated();
            var newEntity = new ReviewGood("John Doe") { NumStars = -1 };
            context.Add(newEntity);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var entity = context.ReviewGoods.Single();

            //VERIFY
            entity.VoterName.ShouldEqual("John Doe");
            entity.NumStars.ShouldEqual(-1);
        }

        [Fact]
        public void ReadReviewsGood2WhichCtorUsed()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CtorDbContext>();
            using var context = new CtorDbContext(options, false);
            context.Database.EnsureCreated();
            var newEntity = new ReviewGood2("John Doe",1);
            context.Add(newEntity);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var entity = context.Review2Goods.Single();

            //VERIFY
            entity.VoterName.ShouldEqual("John Doe");
            entity.NumStars.ShouldEqual(1);
        }

        [Fact]
        public void ReadReviewBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CtorDbContext>();
            using var context = new CtorDbContext(options, false);
            context.Database.EnsureCreated();
            var newEntity = new ReviewBad("John Doe") {NumStars = -1};
            newEntity.VoterName.ShouldEqual("Name: John Doe");
            context.Add(newEntity);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var entity = context.ReviewBads.Single();

            //VERIFY
            entity.VoterName.ShouldEqual("Name: Name: John Doe");
            entity.NumStars.ShouldEqual(-1);
        }

        [Fact]
        public void ReadReviewBadCtor2()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CtorDbContext>();
            using var context = new CtorDbContext(options, false);
            context.Database.EnsureCreated();
            var newEntity = new ReviewBadCtor2("John Doe", -1);
            newEntity.VoterName.ShouldEqual("John Doe");
            context.Add(newEntity);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var entity = context.ReviewBadCtor2s.Single();

            //VERIFY
            entity.VoterName.ShouldEqual("John Doe");
            entity.NumStars.ShouldEqual(-1);
        }

        //Has to be run on its own as it changes the DbContext configuration
        [RunnableInDebugOnly]
        public void ReadReviewBadCtor1()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<CtorDbContext>();
            using var context = new CtorDbContext(options, true);
            //ATTEMPT
            var ex = Assert.Throws<InvalidOperationException>(() => context.Database.EnsureCreated());

            //VERIFY
            ex.Message.ShouldStartWith("No suitable constructor found for entity type 'ReviewBadCtor1'.");
        }
    }
}