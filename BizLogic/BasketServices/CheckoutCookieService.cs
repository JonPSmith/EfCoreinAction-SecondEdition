// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using BizLogic.Orders;

namespace BizLogic.BasketServices
{
    public class CheckoutCookieService
    {
        private List<OrderLineItem> _lineItems;

        public CheckoutCookieService(string cookieContent) //#A
        {                                                  //#A
            DecodeCookieString(cookieContent);             //#A
        }                                                  //#A

        public Guid UserId { get; private set; } //#B
        public ImmutableList<OrderLineItem> LineItems => _lineItems.ToImmutableList(); //#C

        public void AddLineItem(OrderLineItem newItem) //#D
        {                                              //#D
            _lineItems.Add(newItem);                   //#D
        }                                              //#D

        public void DeleteLineItem(int itemIndex)                        //#E
        {                                                                //#E
            if (itemIndex <0 || itemIndex > _lineItems.Count)            //#E
                throw new                                                //#E
                    InvalidOperationException("Couldn't find that item"); //#E
            _lineItems.RemoveAt(itemIndex);                              //#E
        }                                                                //#E

        public void ClearAllLineItems()  //#F
        {                                //#F
            _lineItems.Clear();          //#F
        }                                //#F

        public string EncodeForCookie()                  //#G
        {                                                //#G
            var sb = new StringBuilder();                //#G
            sb.Append(UserId.ToString("N"));             //#G
            foreach (var lineItem in _lineItems)         //#G
            {                                            //#G
                sb.AppendFormat(",{0},{1}",              //#G
                    lineItem.BookId, lineItem.NumBooks); //#G
            }                                            //#G
            return sb.ToString();                        //#G
        }                                                //#G
        /*********************************************************
        #A When you create the CheckoutCookieService you decode the string that came from the cookie
        #B Because the Book App doesn't ask you to log in, you create a unique user Id which we add to the cookie 
        #C The LineItem contains a list of OrderLineItem which each have a BookId and the quantity of that book the user wants to buy 
        #D This method adds a new OrderLineItem entry, which will be encoded into the basket Cookie by the ASP.NET Core's action 
        #E This method allows the user to remove a specific book/quantity order
        #F This method is called to clear the basket cookie of all of the book/quantity entries when the Buy action was successful
        #G This method is used by the ASP.NET Core's actions to update the cookie with the changed book/quantity entries
         * ******************************************************/

        //---------------------------------------------------
        //private methods

        private void DecodeCookieString(string cookieContent)
        {
            _lineItems = new List<OrderLineItem>();

            if (cookieContent == null)
            {
                //No cookie exists, so create new user and no orders
                UserId = Guid.NewGuid();
                return;
            }

            //Has cookie, so decode it
            var parts = cookieContent.Split(',');
            UserId = Guid.Parse(parts[0]);
            for (int i = 0; i < (parts.Length - 1) / 2; i++)
            {
                _lineItems.Add(new OrderLineItem
                {
                    BookId = int.Parse(parts[i * 2 + 1]),
                    NumBooks = short.Parse(parts[i * 2 + 2])
                });
            }
        }
    }
}