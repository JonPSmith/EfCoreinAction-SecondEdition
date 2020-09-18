// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.Dtos;
using Microsoft.AspNetCore.Html;
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
                BookId             = p.BookId,
                Title              = p.Title,
                PublishedOn        = p.PublishedOn,
                EstimatedDate      = p.EstimatedDate,
                OrgPrice           = p.OrgPrice,
                ActualPrice        = p.ActualPrice,
                PromotionText      = p.PromotionalText,
                AuthorsOrdered     = string.Join(", ",
                    p.AuthorsLink
                        .OrderBy(q => q.Order)
                        .Select(q  => q.Author.Name)),
                TagStrings         = p.Tags.Select(x => x.TagId).ToArray(),
                ImageUrl           = p.ImageUrl,
                ManningBookUrl     = p.ManningBookUrl,
                Description        = new HtmlString(p.Details.Description),
                AboutAuthor        = new HtmlString(p.Details.AboutAuthor),
                AboutReader        = new HtmlString(p.Details.AboutReader),
                AboutTechnology    = new HtmlString(p.Details.AboutTechnology),
                WhatsInside        = new HtmlString(p.Details.WhatsInside)
            }).SingleOrDefaultAsync(x => x.BookId == bookId);
        }
    }
}