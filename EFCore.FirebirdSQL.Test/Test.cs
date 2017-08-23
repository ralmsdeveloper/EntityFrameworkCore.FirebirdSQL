 
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace EFCore.FirebirdSqlSQL.Test
{
    public class Context : DbContext
    {
        
        public DbSet<Blog> Blog { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { 

            string connectionString =
            "User=SYSDBA;" +
            "Password=masterkey;" +
            $"Database={AppContext.BaseDirectory}Rafael.fdb;" +
            "DataSource=localhost;" +
            "Port=2017;" +
            "Dialect=3;" +
            "Charset=NONE;" +
            "Role=;" +
            "Connection lifetime=15;" +
            "Pooling=true;" +
            "MinPoolSize=1;" +
            "MaxPoolSize=50;" +
            "Packet Size=8192;" +
            "ServerType=0";

            optionsBuilder.UseFirebirdSql(connectionString);

            base.OnConfiguring(optionsBuilder);
            LoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new TraceLoggerProvider());
            optionsBuilder.UseLoggerFactory(loggerFactory);

        }
        protected override void OnModelCreating(ModelBuilder modelo)
        {
            //Fluent Api
            modelo.Entity<Blog>(entity =>
            {
                entity.HasIndex(e => e.BlogId)
                    .HasName("Id")
                    .IsUnique();
            });
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; } 
        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    //public partial class Blog
    //{
    //    [Key]
    //    public int Id { get; set; }
    //    public int Access { get; set; }
    //    [StringLength(20)]
    //    public string Description { get; set; }
    //    [StringLength(500)]
    //    public string Observations { get; set; }
    //    public DateTime Date { get; set; }

    //} 
    
}

