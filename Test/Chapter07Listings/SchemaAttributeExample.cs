// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter07Listings
{
    [Table("SchemaAttribute", Schema = "Schema1")]
    public class SchemaAttributeExample
    {
        public int Id { get; set; }
    }
}