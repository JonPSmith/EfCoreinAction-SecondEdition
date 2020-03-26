// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BizLogic.GenericInterfaces;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.BizRunners
{
    public class RunnerWriteDbAsync<TIn, TOut>
    {
        private readonly IBizActionAsync<TIn, TOut> _actionClass;
        private readonly DbContext _context;

        public RunnerWriteDbAsync(IBizActionAsync<TIn, TOut> actionClass, DbContext context)
        {
            _context = context;
            _actionClass = actionClass;
        }

        public IImmutableList<ValidationResult> Errors => _actionClass.Errors;
        public bool HasErrors => _actionClass.HasErrors;

        public async Task<TOut> RunActionAsync(TIn dataIn)
        {
            var result = await _actionClass.ActionAsync(dataIn).ConfigureAwait(false);
            if (!HasErrors)
                await _context.SaveChangesAsync();

            return result;
        }
    }
}