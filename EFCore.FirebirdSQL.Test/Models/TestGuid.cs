using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.FirebirdSQL.Test.Models
{
    public partial class TestGuid
    {
        [Column(TypeName = "CHAR(16)")]
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
    }
}
