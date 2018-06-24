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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FB = FirebirdSql.Data.FirebirdClient;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public class TestBasic
    {
        private TestContext CreateContext() => new TestContext();

        [Fact]
        public void ReproIssue28()
        {
            using (var ctx = new Issue28Context())
            {
                ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
                ctx.People.Add(new People
                {
                    Givenname = "Test",
                    Name = "Ralms"
                });
                ctx.SaveChanges();

                var peoples = ctx
                    .People
                    .AsNoTracking()
                    .Where(p => p.Id > 0)
                    .ToList();

                Assert.Single(peoples);
            }
        }

        [Fact]
        public void Insert_data()
        {
            using (var context = CreateContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // Repro Issue #23
                if (!context.Courses.Any())
                {
                    var courses = new[]
                    {
                        new Course{CourseID=1050,Credits=3,Title="Chemistry"},
                        new Course{CourseID=4022,Credits=3,Title="Microeconomics"},
                        new Course{CourseID=4041,Credits=3,Title="Macroeconomics"},
                        new Course{CourseID=1045,Credits=4,Title="Calculus"},
                        new Course{CourseID=3141,Credits=4,Title="Trigonometry"},
                        new Course{CourseID=2021,Credits=3,Title="Composition"},
                        new Course{CourseID=2042,Credits=4,Title="Literature"}
                    };

                    context.Courses.AddRange(courses);
                    var save = context.SaveChanges();
                    Assert.Equal(7, save);
                }
            }

            using (var context = CreateContext())
            {
                // Issue #21
                context.Author.Any();

                for (var i = 1; i <= 10; i++)
                {
                    var author = new Author
                    {
                        TestString = "EFCore FirebirdSQL 2.x",
                        TestInt = i,
                        TestDate = DateTime.Now,
                        TestGuid = Guid.NewGuid(),
                        TestBytes = Encoding.UTF8.GetBytes("EntityFrameworkCore.FirebirdSQL"),
                        TestDecimal = i,
                        TestDouble = i,
                        TimeSpan = DateTime.Now.TimeOfDay,
                        Active = i % 2 == 0
                    };
                    var book = new Book
                    {
                        Title = $"Firebird 3.0.2 {i}"
                    };
                    author.Books.Add(new BookAuthor() {
                        Book = book,
                        Author = author
                    });
                    context.Author.Add(author);
                }
                var save = context.SaveChanges();
                Assert.Equal(30, save);

                for (var i = 1; i <= 10; i++)
                {
                    var author = context.Author.FirstOrDefault(p => p.AuthorId == i);
                    author.TestString = "EFCore FirebirdSQL 2.1-rc1";
                    context.Author.Attach(author);
                }
                var update = context.SaveChanges();
                Assert.Equal(10, update);

                var select = context
                    .Author
                    .Select(x=> x.TestDate.TimeOfDay)
                    .ToList();
            }

            using (var context = CreateContext())
            {
                for (var i = 1; i <= 10; i++)
                {
                    context.Person.Add(new Person
                    {
                        Name = "Rafael",
                        LastName = $"Almeida {i}"
                    });
                }

                Assert.Equal(10, context.SaveChanges());
            }

            using (var context = CreateContext())
            {
                for (var i = 1; i <= 10; i++)
                {
                    var book = new Book
                    {
                        Title = $"Test Insert Book {i}"
                    };
                    book.Authors.Add(new BookAuthor() {
                        Author = context.Author.Find((long)i),
                        Book = book
                    });
                    context.Book.Add(book);
                }
                Assert.Equal(20, context.SaveChanges());
            }
        }
    }
}
