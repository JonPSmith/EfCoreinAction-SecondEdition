// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BizLogic.GenericInterfaces
{
    public interface IBizActionAsync<in TIn, TOut>
    {
        IImmutableList<ValidationResult> Errors { get; }

        bool HasErrors { get; }

        Task<TOut> ActionAsync(TIn dto);
    }
}