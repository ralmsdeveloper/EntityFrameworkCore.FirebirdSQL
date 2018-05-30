/*
 *          Copyright (c) 2017-2018 Rafael Almeida (ralms@ralms.net)
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
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using FB = FirebirdSql.Data.FirebirdClient;

namespace EFCore.FirebirdSql.FunctionalTests
{
    // Repro: https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL/issues/28
    public class Issue28Context : DbContext
    {
        public virtual DbSet<People> People { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new FB.FbConnectionStringBuilder(
               @"User=SYSDBA;Password=masterkey;Database=..\..\..\Issue28.fdb;DataSource=localhost;Port=3050;")
                {
                  // Dialect = 1
                }.ConnectionString;

            optionsBuilder
                .UseFirebird(connectionString)
                .ConfigureWarnings(c => c.Log(CoreEventId.IncludeIgnoredWarning));

            var loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();

            optionsBuilder.UseLoggerFactory(loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<People>(entity =>
            {
                entity.ToTable("PEOPLE");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Givenname)
                    .HasColumnName("GIVENNAME")
                    .HasColumnType("VARCHAR(60)");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasColumnType("VARCHAR(60)");
            });
    }

    public class TestContext : DbContext
    {
        public DbSet<Author> Author { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Person> Person { get; set; }

        public DbSet<Course> Courses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            var connectionString = new FB.FbConnectionStringBuilder(
                @"User=SYSDBA;Password=masterkey;Database=..\..\..\EFCoreSample.fdb;DataSource=localhost;Port=3050;")
                {
                    //Dialect = 1
                }.ConnectionString;

            optionsBuilder
                .UseFirebird(connectionString)
                .ConfigureWarnings(c => c.Log(CoreEventId.IncludeIgnoredWarning));

            var loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();

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
