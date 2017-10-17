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

using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.FirebirdSql.Extensions;
using EntityFrameworkCore.FirebirdSql;
using FirebirdSql.Data.FirebirdClient;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public class TestContext : DbContext
    {
        public DbSet<Author> Author { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Person> Person { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionBuilder =
                new FbConnectionStringBuilder
                {
                    Database = "EfCoreFirebirdSql.fdb",
                    DataSource = "127.0.0.1",
                    Port = 3050,
                    ConnectionTimeout = 10,
                    PacketSize = 4096,
                    Pooling = true,
                    MaxPoolSize = 250,
                    MinPoolSize = 2,
                    UserID = "sysdba",
                    Password = "materkey",
                    ServerType = FbServerType.Default
                };

            optionsBuilder.UseFirebird(connectionBuilder.ToString());
        }

        protected override void OnModelCreating(ModelBuilder modelo)
        {
            base.OnModelCreating(modelo);

            modelo.Entity<Author>()
                .Property(x => x.AuthorId).UseFirebirdSequenceTrigger();

            modelo.Entity<Book>()
                .Property(x => x.BookId).UseFirebirdSequenceTrigger();

            modelo.Entity<Person>()
                .HasKey(person => new { person.Name, person.LastName });
        }
    }
}
