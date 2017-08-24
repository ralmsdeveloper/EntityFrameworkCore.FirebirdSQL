 
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
        
        public DbSet<Product> Products { get; set; } 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { 

            string connectionString =
            "User=SYSDBA;" +
            "Password=masterkey;" +
            $"Database={System.IO.Directory.GetCurrentDirectory()}\\Rafael.fdb;" +
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
            //if used Log
            //LoggerFactory loggerFactory = new LoggerFactory();
            //loggerFactory.AddProvider(new TraceLoggerProvider());
            //optionsBuilder.UseLoggerFactory(loggerFactory);

        }
        protected override void OnModelCreating(ModelBuilder modelo)
        {
            //Fluent Api
            modelo.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("Id");
            });
        }
    }

    public class Product
    {
         
        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public DateTime Update { get; set; } 

        public string Description { get; set; } 
        
        public decimal Price { get; set; }

        public Boolean Locked { get; set; }
    }
     
    
}

