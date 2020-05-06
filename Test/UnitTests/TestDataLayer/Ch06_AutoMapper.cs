// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using ServiceLayer.AdminServices;
using ServiceLayer.BookServices;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_AutoMapper
    {
        private ITestOutputHelper _output;

        public Ch06_AutoMapper(ITestOutputHelper output)
        {
            _output = output;
        }

        public static MapperConfiguration CreateMapperConfig<TSource, TDestination>()
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<TSource, TDestination>());
        }

        public static MapperConfiguration CreateMapperConfigScanTestAutoMap()
        {
            return new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
        }

        [AutoMap(typeof(Book))]
        public class ChangePubDateDtoAm
        {
            public int BookId { get; set; }          
            public string Title { get; set; }        
            [DataType(DataType.Date)]                
            public DateTime PublishedOn { get; set; }
        }

        [Fact]
        public void TestSimpleAutoMapperOk()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.Message);
            });
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var lastBook = context.SeedDatabaseFourBooks().Last();
                var config = CreateMapperConfigScanTestAutoMap();

                //ATTEMPT
                showLog = true;
                var dto = context.Books
                    .ProjectTo<ChangePubDateDtoAm>(config)
                    .Single(x => x.BookId == lastBook.BookId);

                //VERIFY
                dto.BookId.ShouldEqual(lastBook.BookId);
                dto.Title.ShouldEqual(lastBook.Title);
                dto.PublishedOn.ShouldEqual(lastBook.PublishedOn);
            }
        }

        [Fact]
        public void TestSimpleHandCodedOk()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.Message);
            });
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var lastBook = context.SeedDatabaseFourBooks().Last();

                //ATTEMPT
                showLog = true;
                var dto = context.Books
                    .Select(p => new ChangePubDateDto
                    {                                
                        BookId = p.BookId,           
                        Title = p.Title,             
                        PublishedOn = p.PublishedOn  
                    })                               
                    .Single(k => k.BookId == lastBook.BookId);

                //VERIFY
                dto.BookId.ShouldEqual(lastBook.BookId);
                dto.Title.ShouldEqual(lastBook.Title);
                dto.PublishedOn.ShouldEqual(lastBook.PublishedOn);
            }
        }

        public class BookListDtoProfile : Profile                            //#A
        {
            public BookListDtoProfile()
            {
                CreateMap<Book, BookListDto>()                               //#B
                    .ForMember(p => p.ActualPrice,                           //#C
                        m => m.MapFrom(s => s.Promotion == null              //#C
                             ? s.Price : s.Promotion.NewPrice))              //#C
                    .ForMember(p => p.AuthorsOrdered,                        //#D
                        m => m.MapFrom(s => string.Join(", ",                //#D
                                s.AuthorsLink.Select(x => x.Author.Name))))  //#D
                    .ForMember(p => p.ReviewsAverageVotes,                   //#E
                        m => m.MapFrom(s =>                                  //#E
                            s.Reviews.Select(y =>                            //#E
                                (double?)y.NumStars).Average()));            //#E
            }
        }
        /**********************************************************
        #A Your class must inherit the AutoMapper Profile class. You can have multiple classes that inherit Profile 
        #B This is setting up the mapping from the Book entity class to the BookListDto
        #C The Actual price depends on whether the Promotion has a PriceOffer or not
        #D This has the code to get the list of Author's names as a comma delimited string
        #E This contains the special code needed to make the Average method run in the database
         ************************************************/

        public static MapperConfiguration CreateFromProfileConfig()
        {
            return new MapperConfiguration(cfg => cfg.AddProfile<BookListDtoProfile>());
        }

        [Fact]
        public void TestComplexAutoMapperOk()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.Message);
            });
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var lastBook = context.SeedDatabaseFourBooks().Last();
                var config = CreateFromProfileConfig();

                //ATTEMPT
                showLog = true;
                var dto = context.Books
                    .ProjectTo<BookListDto>(config)
                    .Single(x => x.BookId == lastBook.BookId);

                //VERIFY
                dto.BookId.ShouldEqual(lastBook.BookId);
                dto.Title.ShouldEqual(lastBook.Title);
                dto.PublishedOn.ShouldEqual(lastBook.PublishedOn);
                dto.Price.ShouldEqual(lastBook.Price);
                dto.ActualPrice.ShouldEqual(lastBook.Promotion.NewPrice);
                dto.PromotionPromotionalText.ShouldEqual(lastBook.Promotion.PromotionalText);
                dto.AuthorsOrdered.ShouldEqual("Future Person");
                dto.ReviewsCount.ShouldEqual(2);
                dto.ReviewsAverageVotes.ShouldEqual(5.0);
            }
        }
    }
}