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
using EntityFrameworkCore.FirebirdSql;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public class TestContext : DbContext
    {
        public DbSet<Author> Author { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Person> Person { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseFirebird("User=SYSDBA;Password=masterkey;Database=EFCoreSample.fdb;DataSource=localhost;Port=3050;")
                .ConfigureWarnings(c => c.Log(CoreEventId.IncludeIgnoredWarning));

            var loggerFactory = new LoggerFactory().AddConsole().AddDebug();
            optionsBuilder.UseLoggerFactory(loggerFactory);
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
