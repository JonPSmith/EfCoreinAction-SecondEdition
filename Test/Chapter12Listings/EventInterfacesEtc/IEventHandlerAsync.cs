// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Test.Chapter12Listings.EventInterfacesEtc
{
    public interface IEventHandlerAsync<in T> where T : IDomainEvent
    {
        Task HandleEventAsync(T domainEvent);
    }
}