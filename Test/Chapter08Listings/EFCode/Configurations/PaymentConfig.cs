// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Test.Chapter08Listings.EfClasses;

namespace Test.Chapter08Listings.EFCode.Configurations
{
    public class PaymentConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure
            (EntityTypeBuilder<Payment> entity)
        {
            entity.HasDiscriminator(b => b.PType) //#A
                .HasValue<PaymentCash>(PTypes.Cash) //#B
                .HasValue<PaymentCard>(PTypes.Card); //#C
        }
    }
    /*******************************************
    #A The HasDiscriminator method identifies the entity as a TPH and then selects the property PType as the discriminator for the different types. In this case it is an enum, which I have set to be byte in size
    #B This sets the discriminator value for the PaymentCash type
    #C This sets the discriminator value for the PaymentCard type
        * *******************************************/
}