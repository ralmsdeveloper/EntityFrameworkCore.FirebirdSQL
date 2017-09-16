  
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EntityFrameworkCore.FirebirdSql.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace EntityFrameworkCore.FirebirdSql.Console.Test
{
    public class Context : DbContext
    {
        
        public DbSet<Author> Author { get; set; }
        public DbSet<Book> Book { get; set; } 
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {  

            string connectionString =
            "User=SYSDBA;" +
            "Password=#j@ms0ft;" +
            $"Database=localhost:{System.IO.Directory.GetCurrentDirectory()}\\FirebirdCore.fdb;" +
            "DataSource=localhost;" +
            "Port=2017;"+
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
            LoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new TraceLoggerProvider());
            optionsBuilder.UseLoggerFactory(loggerFactory);
        }
        protected override void OnModelCreating(ModelBuilder modelo)
        {}
    } 

    public class Author
    {
        public long AuthorId { get; set; }

        [StringLength(100)]
        public string TestString { get; set; } 

        public DateTime TestDate { get; set; }
         
        public Guid TestGuid{ get; set; }

		public byte[] TestBytes { get; set; }

	    public int TestInt { get; set; }

	    public decimal TestDecimal { get; set; }

	    public double TestDouble { get; set; }

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
}

