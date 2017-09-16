using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;
namespace EntityFrameworkCore.FirebirdSql.Console.Test
{
	public class Program
	{ 
		static void Main(string[] args)
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
				for (int i = 0; i < 50; i++)
				{
					cx.Author.Add(new Author
					{
						TestString = "Rafael",
						TestInt = i,
						TestDate = DateTime.Now.AddMilliseconds(1),
						TestGuid = Guid.NewGuid(),
						TestBytes = System.Text.Encoding.UTF8.GetBytes("RAFAEL ALMEIDA"),
						TestDecimal = i,
						TestDouble = i,
						Books = new List<Book>
						{
							new Book {  Title=$"Firebird 3.0.2 {i}"},
							new Book {  Title=$"Firebird 4.0.0 {i}"}
						}
					});
				}
				cx.SaveChanges();
			}


			cx.Book.Add(new Book
			{
				AuthorId = 1,
				Title = "Test of Test"
			});
			cx.SaveChanges();


			var AuthorsUpdate1 = cx.Author.Find((long)1);
			cx.Attach(AuthorsUpdate1);
			AuthorsUpdate1.TestString = $"Author Modified {Guid.NewGuid()}";

			var AuthorsUpdate2 = cx.Author.Find((long)2);
			cx.Attach(AuthorsUpdate2);
			AuthorsUpdate2.TestString = $"Author Modified {Guid.NewGuid()}";

			var AuthorsUpdate3 = cx.Author.Find((long)3);
			cx.Attach(AuthorsUpdate3);
			AuthorsUpdate3.TestString = $"Author Modified {Guid.NewGuid()}";

			cx.SaveChanges();


			var obj = cx.Author.Find((long)1);
			cx.Author.Remove(obj);
			cx.SaveChanges();

			var y = cx.Author.Include(p => p.Books).Where(p => p.TestString.Trim() != "A").ToList();
			var Authors = cx.Author.AsNoTracking()
							.Include(p => p.Books).Where(p => p.AuthorId < 10)
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
