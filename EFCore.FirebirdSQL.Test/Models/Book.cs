using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.FirebirdSQL.Test.Models
{
    public partial class Book
    {
        public long BookId { get; set; }
        public long AuthorId { get; set; }
        [Required]
        public string Title { get; set; }

        [ForeignKey("AuthorId")]
        [InverseProperty("Book")]
        public Author Author { get; set; }
    }
}
