// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BookApp.ServiceLayer.DefaultSql.Books.Services
{
    public class DetailBookService : IDetailBookService
    {
        private readonly BookDbContext _context;

        public DetailBookService(BookDbContext context)
        {
            _context = context;
        }

        public Task<BookDetailDto> GetBookDetailAsync(int bookId)
        {
            return  _context.Books.Select(p => new BookDetailDto
            {
                BookId = p.BookId,
                Title = p.Title,
                PublishedOn = p.PublishedOn,
                EstimatedDate = p.EstimatedDate,
                OrgPrice = p.OrgPrice,
                ActualPrice = p.ActualPrice,
                PromotionText = p.PromotionalText,
                AuthorsOrdered = string.Join(", ",
                    p.AuthorsLink
                        .OrderBy(q => q.Order)
                        .Select(q => q.Author.Name)),
                TagStrings = p.TagsLink.Select(x => x.TagId).ToArray(),
                ImageUrl = p.ImageUrl,
                ManningBookUrl = p.ManningBookUrl,
                Description = p.Description,
                AboutAuthor = p.AboutAuthor,
                AboutReader = p.AboutReader,
                AboutTechnology = p.AboutTechnology,
                WhatsInside =  p.WhatsInside
            }).SingleOrDefaultAsync(x => x.BookId == bookId);
        }
    }
}