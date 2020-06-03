// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Chapter08Listings.EfClasses
{
    public class DeletePrincipal
    {
        public int DeletePrincipalId { get; set; }

        public DeleteDependentDefault DependentDefault { get; set; }

        public DeleteDependentSetNull DependentSetNull { get; set; }

        public DeleteDependentRestrict DependentRestrict { get; set; }

        public DeleteDependentCascade DependentCascade { get; set; }

        public DeleteNonNullDefault DependentNonNullDefault { get; set; }
    }
}