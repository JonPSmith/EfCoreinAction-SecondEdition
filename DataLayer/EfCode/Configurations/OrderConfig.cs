// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataLayer.EfCode.Configurations
{
    internal class OrderConfig : IEntityTypeConfiguration<Order>
    {
        private readonly Guid _userId;

        public OrderConfig(Guid userId)
        {
            _userId = userId;
        }

        public void Configure(EntityTypeBuilder<Order> entity)
        {
            entity.HasQueryFilter(x => x.CustomerId == _userId);
        }
    }
}