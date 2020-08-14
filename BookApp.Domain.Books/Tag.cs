// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace BookApp.Domain.Books
{
    public class Tag
    {
        public Tag(string tagId)
        {
            TagId = tagId;
        }

        [Key]
        [Required]
        [MaxLength(100)]
        public string TagId { get; private set; }
    }
}