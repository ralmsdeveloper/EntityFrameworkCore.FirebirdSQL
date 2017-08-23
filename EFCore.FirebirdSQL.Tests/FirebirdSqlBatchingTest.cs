using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace EFCore.FirebirdSQL.Tests
{
    [TestClass]
    public class FirebirdSqlBatchingTest : IDisposable
    {
 
        [TestMethod]
        public void Insert()
        {
            var optionsBuilder = new DbContextOptionsBuilder(); 
            var expectedBlogs = new List<Blog>();
            using (var context = new BloggingContext())
            {
                context.Database.EnsureCreated();
                var owner1 = new Post();
                var owner2 = new Post();
                context.Posts.Add(owner1);
                context.Posts.Add(owner2);

                for (var i = 1; i < 100; i++)
                {
                    var blog = new Blog
                    {
                        Post = i % 2 == 0 ? owner1 : owner2
                    }; 
                    context.Blogs.Add(blog);
                    expectedBlogs.Add(blog);
                }

                context.SaveChanges();
            }

            expectedBlogs =  expectedBlogs.OrderBy(b => b.Id).ToList();
            using (var context = new BloggingContext())
            {
                var actualBlogs = expectedBlogs.OrderBy(b => b.Id).ToList();
                Xunit.Assert.Equal(expectedBlogs.Count, actualBlogs.Count);

                
                for (var i = 0; i < actualBlogs.Count; i++)
                {
                    var expected = expectedBlogs[i];
                    var actual = actualBlogs[i];
                    Xunit.Assert.Equal(expected.Id, actual.Id);
                    Xunit.Assert.Equal(expected.Order, actual.Order);
                    Xunit.Assert.Equal(expected.PostId, actual.PostId); 
                }
            }
        }

        private class BloggingContext : DbContext
        {
            public BloggingContext()
            {
            }
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                var obj = new FirebirdSqlTestStore();
                optionsBuilder.UseFirebirdSql(obj.ConnectionString); 
                base.OnConfiguring(optionsBuilder);
            }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            { 
            }

            public DbSet<Blog> Blogs { get; set; }
            public DbSet<Post> Posts { get; set; }
        }

        private class Blog
        {
            [Key]
            public int Id { get; set; }
            public int Order { get; set; }
            public int? PostId { get; set; }
            public Post Post { get; set; } 
        }

        public class Post
        {
            public int Id { get; set; }
        }

        private readonly FirebirdSqlTestStore _testStore;
        private readonly IServiceProvider _serviceProvider;

        public FirebirdSqlBatchingTest()
        {
            _testStore = new FirebirdSqlTestStore();
            _serviceProvider = new ServiceCollection()
                .AddEntityFrameworkFirebirdSql()
                .BuildServiceProvider();
        }

        public void Dispose() => _testStore.Dispose();
    }
}
