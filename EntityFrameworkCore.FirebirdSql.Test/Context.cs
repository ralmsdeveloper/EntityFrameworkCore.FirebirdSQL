/*
 *          Copyright (c) 2017 Rafael Almeida (ralms@ralms.net)
 *
 *                    EntityFrameworkCore.FirebirdSql
 *
 * THIS MATERIAL IS PROVIDED AS IS, WITH ABSOLUTELY NO WARRANTY EXPRESSED
 * OR IMPLIED.  ANY USE IS AT YOUR OWN RISK.
 * 
 * Permission is hereby granted to use or copy this program
 * for any purpose,  provided the above notices are retained on all copies.
 * Permission to modify the code and to distribute modified code is granted,
 * provided the above notices are retained, and a notice that the code was
 * modified is included with the above copyright notice.
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkCore.FirebirdSql.Extensions;
using Microsoft.EntityFrameworkCore;  

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
            "Password=masterkey;" +
            $"Database=localhost:{System.IO.Directory.GetCurrentDirectory()}\\FirebirdCore.fdb;" +
            "DataSource=localhost;" +
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
        public string TestString { get; set; } 

        public DateTime TestDate { get; set; }

	    [Column(TypeName = "CHAR(16)")]
		public string TestGuid { get; set; }

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

