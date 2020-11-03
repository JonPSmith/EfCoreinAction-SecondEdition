// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace BookApp.Domain.Books.SupportTypes
{
    public interface ICreatedUpdated
    {
        DateTime WhenCreatedUtc { get; }
        DateTime LastUpdatedUtc { get; }
        bool NotUpdatedYet { get; }

        /// <summary>
        /// This should be called if the status of the entity is Added or Modified
        /// You check this just before SaveChanges/Async is called, typically by overriding SaveChanges/Async
        /// </summary>
        /// <param name="added"></param>
        void LogAddUpdate(bool added);
    }
}