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
using System.Linq;
using System.Text;
using Xunit;

namespace EFCore.FirebirdSql.FunctionalTests
{
    public class TestBasic
    {
        private TestContext CreateContext() => new TestContext();

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

                for (var i = 1; i <= 100; i++)
                {
                    context.Author.Add(new Author
                    {
                        TestString = "EFCore FirebirdSQL 2.x",
                        TestInt = i,
                        TestDate = DateTime.Now.AddMilliseconds(1),
                        TestGuid = Guid.NewGuid(),
                        TestBytes = Encoding.UTF8.GetBytes("EntityFrameworkCore.FirebirdSQL"),
                        TestDecimal = i,
                        TestDouble = i,
                        Books = new List<Book>
                        {
                            new Book
                            {
                                AuthorId= i,
                                Title = $"Firebird 3.0.2 {i}"
                            }
                        }
                    });
                }
                var save = context.SaveChanges();
                Assert.Equal(200, save);

                for (var i = 1; i <= 100; i++)
                {
                    var author = context.Author.Find((long)i);
                    author.TestString = "EFCore FirebirdSQL Preview1";
                    context.Author.Attach(author);
                }
                var update = context.SaveChanges();
                Assert.Equal(100, update);

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
                    context.Book.Add(new Book
                    {
                        AuthorId = i,
                        Title = $"Test Insert Book {i}"
                    });
                }

                Assert.Equal(10, context.SaveChanges());
            }
        }

        [Fact]
        public void Delete_data()
        {
            var removed = 0;
            using (var context = CreateContext())
            {
                for (long i = 1; i <= 100; i++)
                {
                    var author = context.Author.Find(i);
                    context.Author.Remove(author);
                    removed += context.SaveChanges();
                }
            }
            Assert.Equal(100, removed);
        }
    }
}
