 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FirebirdSql.EntityFrameworkCore.Firebird.Test
{
    public class Context : DbContext
    {
        
        public DbSet<Author> Author { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Test> Test { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {  

            string connectionString =
            "User=SYSDBA;" +
            "Password=masterkey;" +
            $"Database={System.IO.Directory.GetCurrentDirectory()}\\FirebirdCore.fdb;" +
            "DataSource=127.0.0.1;" +
            "Port=3050;"+
            "Dialect=3;" +
            "Charset=NONE;" +
            "Role=;" +
            "Connection lifetime=15;" +
            "Pooling=true;" +
            "MinPoolSize=1;" +
            "MaxPoolSize=50;" +
            "Packet Size=8192;" +
            "ServerType=0"; 
            optionsBuilder.UseFirebird(connectionString); 

        }
        protected override void OnModelCreating(ModelBuilder modelo)
        {}
    } 

    public class Author
    {
        public long AuthorId { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        public DateTime Date { get; set; }
         
        public Guid Identification { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }

    public class Book
    {
        public long BookId { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        public long AuthorId { get; set; }

        public Author Author { get; set; }
    }

    public class Test
    {
        public Guid  Id { get; set; } 

        [StringLength(100)]
        public string Description { get; set; } 
    }
}

