// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfCode;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.QueryObjects;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayer
{
    public class Ch02_ListSortFilterPageDto
    {
        [Theory]
        [InlineData(BooksFilterBy.ByVotes, "Dummy", 10, 2, 2)]
        [InlineData(BooksFilterBy.ByPublicationYear, "2010", 5, 1, 3)]
        public void SetupRestOfDto(BooksFilterBy filterBy, string filterValue, int pageSize,
            int expectedPageNum, int expectedNumPages)
        {
            //SETUP
            var numBooks = 12;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(numBooks);

                var sfpDto = new SortFilterPageOptions
                {
                    FilterBy = BooksFilterBy.ByVotes,
                    FilterValue = "Dummy",
                    PageNum = 2
                };

                //need to do this to to setup PrevCheckState 
                sfpDto.SetupRestOfDto(context.Books);

                //ATTEMPT
                sfpDto.PageNum = 2;
                sfpDto.FilterBy = filterBy;
                sfpDto.FilterValue = filterValue;
                sfpDto.PageSize = pageSize;
                sfpDto.SetupRestOfDto(context.Books);

                //VERIFY
                sfpDto.PageNum.ShouldEqual(expectedPageNum);
                sfpDto.NumPages.ShouldEqual(expectedNumPages);
            }
        }

        [Fact]
        public void DefaultValues()
        {
            //SETUP

            //ATTEMPT
            var sfpDto = new SortFilterPageOptions();

            //VERIFY
            sfpDto.OrderByOptions.ShouldEqual(OrderByOptions.SimpleOrder);
            sfpDto.FilterBy.ShouldEqual(BooksFilterBy.NoFilter);
            sfpDto.FilterValue.ShouldBeNull();
            sfpDto.PageNum.ShouldEqual(1);
            sfpDto.PageSize.ShouldEqual(SortFilterPageOptions.DefaultPageSize);
            sfpDto.NumPages.ShouldEqual(0);
            sfpDto.PrevCheckState.ShouldBeNull();
        }
    }
}