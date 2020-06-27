// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace ServiceLayer.SoftDeleteServices.Concrete
{
    public class CascadeSoftDeleteInfo
    {

        public CascadeSoftDeleteInfo(CascadeSoftDelWhatDoing whatDoing, string error)
        {
            WhatDoing = whatDoing;
            Error = error;
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

        /// <summary>
        /// If there is an error, then this contains a error massage. Otherwise it is null
        /// </summary>
        public string Error { get; }

        public override string ToString()
        {
            if (Error != null)
                return Error;

            switch (WhatDoing)
            {
                case CascadeSoftDelWhatDoing.SoftDelete:
                    return Message("soft deleted");
                case CascadeSoftDelWhatDoing.UnSoftDelete:
                    return Message("un-soft deleted");
                case CascadeSoftDelWhatDoing.CheckWhatWillDelete:
                {
                    if (NumFound == 0)
                        return $"No entries will be hard deleted";
                    return $"Are you sure you want to hard delete this entity{DependentsSuffix()}";
                }
                case CascadeSoftDelWhatDoing.HardDeleteSoftDeleted:
                    return Message("hard deleted");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string Message(string what)
        {
            if (NumFound == 0)
                return $"No entries have been {what}";
            var dependentsSuffix = NumFound > 1
                ? $" and its {NumFound - 1} dependents"
                : "";
            return $"You have {what} an entity{DependentsSuffix()}";
        }

        private string DependentsSuffix()
        {
            return NumFound > 1
                ? $" and its {NumFound - 1} dependents"
                : "";
        }
    }
}