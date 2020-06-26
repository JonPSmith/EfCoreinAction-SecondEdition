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
                    return Message("hard delete", false);
                case CascadeSoftDelWhatDoing.HardDeleteSoftDeleted:
                    return Message("hard deleted");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string Message(string what, bool done = true)
        {
            var wereWould = done ? "were" : "would";
            var haveWould = done ? "have" : "would";
            if (NumFound == 0)
                return $"No entries {wereWould} {what}";

            var dependentsSuffix = NumFound > 1
                ? $" and its {NumFound - 1} dependents"
                : "";
            return $"You {haveWould} {what} an entity{dependentsSuffix}";
        }
    }
}