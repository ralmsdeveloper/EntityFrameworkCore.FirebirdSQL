using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.FirebirdSQL.Test.Models
{
    public partial class Author
    {
        public Author()
        {
            Book = new HashSet<Book>();
        }

        public long AuthorId { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime Date { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [InverseProperty("Author")]
        public ICollection<Book> Book { get; set; }
    }
}
