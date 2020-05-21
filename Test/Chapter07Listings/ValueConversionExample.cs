// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter07Listings
{
    public enum Stages {None, One, Two, Three}

    public class ValueConversionExample
    {
        public int Id { get; set; }

        public Stages Stage { get; set; }

        [Column(TypeName = "varchar(5)")]
        public Stages StageViaAttribute { get; set; }

        public Stages StageViaFluent { get; set; }

        public Stages? StageCanBeNull { get; set; }

        public DateTime DateTimeUtc { get; set; }

        public DateTime DateTimeUtcUtcOnReturn { get; set; }
        public DateTime DateTimeUtcSaveAsString { get; set; }
    }
}