// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace ServiceLayer.SoftDeleteServices.Concrete
{
    public class CascadeSoftDeleteInfo
    {

        public CascadeSoftDeleteInfo(CascadeSoftDelWhatDoing whatDoing)
        {
            WhatDoing = whatDoing;
            NumFound = 0;
        }

        /// <summary>
        /// Number of entities visited
        /// </summary>
        public int NumFound { get; internal set; }

        /// <summary>
        /// What service you where using
        /// </summary>
        public CascadeSoftDelWhatDoing WhatDoing { get; }

        public override string ToString()
        {
            switch (WhatDoing)
            {
                case CascadeSoftDelWhatDoing.SoftDelete:
                    return Message("soft deleted");
                case CascadeSoftDelWhatDoing.UnSoftDelete:
                    return Message("un-soft deleted");
                case CascadeSoftDelWhatDoing.CheckWhatWillDelete:
                    return Message("hard deleted", true);
                case CascadeSoftDelWhatDoing.HardDeleteSoftDeleted:
                    return Message("hard deleted");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string Message(string what, bool preCheck = false)
        {
            var haveWould = preCheck ? "would be" : "have been" ;
            if (NumFound == 0)
                return $"No entries {haveWould} {what}";

            var wereWould = preCheck ? "would" : "have";
            var dependentsSuffix = NumFound > 1
                ? $" and its {NumFound - 1} dependents"
                : "";
            return $"You {wereWould} {what} an entity{dependentsSuffix}";
        }
    }
}