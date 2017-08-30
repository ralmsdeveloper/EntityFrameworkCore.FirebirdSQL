using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EFCore.FirebirdSQL.Test.Models
{
    public partial class FirebirdDbContext : DbContext
    {
        public virtual DbSet<Author> Author { get; set; }
        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<TestGuid> TestGuid { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseFirebirdSql(@"User=SYSDBA;Password=masterkey;Database=R:\RALMS.FDB;DataSource=127.0.0.1;Port=2017;Dialect=3;Charset=UTF8;Role=;Connection lifetime=15;Pooling=true;Packet Size=8192;ServerType=0;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(entity =>
            {
                entity.Property(e => e.AuthorId).ValueGeneratedNever();
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.BookId).ValueGeneratedNever();

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Book)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<TestGuid>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });
        }
    }
}
