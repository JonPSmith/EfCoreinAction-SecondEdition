// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace DataLayer.Interfaces
{
    public interface ICascadeSoftDelete
    {
        public byte SoftDeleteLevel { get; set; }
    }
}