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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.Console;

namespace EntityFrameworkCore.FirebirdSql.Console.Test
{
	public class Program
	{
		private static void Main(string[] args)
		{
			//Command Sample Scaffolding
			//Scaffold-DbContext "User=SYSDBA;Password=masterkey;Database=C:\FirebirdEFCore.FDB;DataSource=127.0.0.1;Port=3050;Dialect=3;Charset=UTF8;Role=;Connection lifetime=15;Pooling=true;Packet Size=8192;ServerType=0;" EntityFrameworkCore.FirebirdSql -OutputDir Models -Context "FirebirdDbContext" -DataAnnotations -force -verbose
			WriteLine("# Wait... ");
			var cx = new Context();
			WriteLine("# Deleting database...\n");
			cx.Database.EnsureDeleted();
			cx.Database.EnsureCreated();

			//Sample (https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSql/issues/3)
			if (!cx.Author.Any())
			{
				////fifty rows
				for (var i = 0; i < 50; i++)
					cx.Author.Add(new Author
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
			//Add Person
			var person = new Person
			{
				Name = "Rafael",
				LastName = "Santos"
			};
			cx.Person.Add(person);
			cx.SaveChanges();

			//Remove Person
			cx.Person.Remove(person);
			cx.SaveChanges();

			cx.Book.Add(new Book
			{
				AuthorId = 1,
				Title = "Test of Test"
			});
			cx.SaveChanges();


			var AuthorsUpdate1 = cx.Author.Find((long) 1);
			cx.Attach(AuthorsUpdate1);
			AuthorsUpdate1.TestString = $"Author Modified {Guid.NewGuid()}";

			var AuthorsUpdate2 = cx.Author.Find((long) 2);
			cx.Attach(AuthorsUpdate2);
			AuthorsUpdate2.TestString = $"Author Modified {Guid.NewGuid()}";

			var AuthorsUpdate3 = cx.Author.Find((long) 3);
			cx.Attach(AuthorsUpdate3);
			AuthorsUpdate3.TestString = $"Author Modified {Guid.NewGuid()}";

			cx.SaveChanges();


			var obj = cx.Author.Find((long) 1);
			cx.Author.Remove(obj);
			cx.SaveChanges();

			var y = cx.Author.Include(p => p.Books).Where(p => p.TestString.Trim() != "A").ToList();
			var Authors = cx.Author.AsNoTracking()
			                .Include(p => p.Books)
			                .Where(p => p.AuthorId < 10)
			                .ToList();

			WriteLine($"-----------------------------------");
			foreach (var item in Authors)
			{
				WriteLine($"Author #->{item.TestString}  ");

				WriteLine($"--------------BOOKS----------------");
				foreach (var book in item.Books)

					WriteLine($"Book: {book.Title}");


				WriteLine($"-----------------------------------");
			}

			ReadKey();
		}
	}
}
