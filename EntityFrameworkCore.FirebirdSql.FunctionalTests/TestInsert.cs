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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace EntityFrameworkCore.FirebirdSql.FunctionalTests
{
    public class TestInsert
    {
		private TestContext CreateContext() => new TestContext();

		[Fact]
		public async Task insert_data()
		{
			using (var context = CreateContext())
			{
				context.Database.EnsureDeleted();
				context.Database.EnsureCreated();
			}

			using (var context = CreateContext())
			{
				for (var i = 0; i < 1000; i++)
				{
					context.Author.Add(new Author
					{
						TestString = "Rafael",
						TestInt = i,
						TestDate = DateTime.Now.AddMilliseconds(1),
						TestGuid = Guid.NewGuid(),
						TestBytes = Encoding.UTF8.GetBytes("RAFAEL ALMEIDA"),
						TestDecimal = i,
						TestDouble = i,
						Books = new List<Book>
						{
							new Book {Title = $"Firebird 3.0.2 {i}"},
							new Book {Title = $"Firebird 4.0.0 {i}"}
						}
					});
				} 
				await context.SaveChangesAsync();
			}

			using (var context = CreateContext())
			{
				for (var i = 0; i < 1000; i++)
				{
					context.Person.Add(new Person
					{
						Name = "Rafael",
						LastName = $"Almeida {i}"
					});
				}
				await context.SaveChangesAsync();
			}

			using (var context = CreateContext())
			{
				for (var i = 0; i < 50; i++)
				{
					context.Book.Add(new Book
					{
						AuthorId = i+1,
						Title = $"Test Insert Book {i}"
					});
				}
				await context.SaveChangesAsync();
			}
			 
			using (var context = CreateContext())
			{
				Assert.Equal(1000, context.Author.Count());
			}
		}
	}
}
