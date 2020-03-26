// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;

namespace ServiceLayer.AdminServices
{
    public interface IChangePubDateService
    {
        ChangePubDateDto GetOriginal(int id);
        Book UpdateBook(ChangePubDateDto dto);
    }
}