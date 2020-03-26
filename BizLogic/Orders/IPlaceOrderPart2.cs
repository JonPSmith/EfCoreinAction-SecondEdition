// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BizLogic.GenericInterfaces;
using DataLayer.EfClasses;

namespace BizLogic.Orders
{
    public interface IPlaceOrderPart2 : IBizAction<Part1ToPart2Dto, Order> {}
}