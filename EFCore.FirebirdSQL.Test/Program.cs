using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityFrameworkCore.FirebirdSQL.Test
{
    class Program
    {

        static void Main(string[] args)
        {
            //Command Sample Scaffolding
            //Scaffold-DbContext "User=SYSDBA;Password=masterkey;Database=C:\FirebirdEFCore.FDB;DataSource=127.0.0.1;Port=3050;Dialect=3;Charset=UTF8;Role=;Connection lifetime=15;Pooling=true;Packet Size=8192;ServerType=0;" EntityFrameworkCore.FirebirdSQL -OutputDir Models -Context "FirebirdDbContext" -DataAnnotations -force -verbose
            Console.WriteLine("# Wait... ");
            var cx = new Context();
            Console.WriteLine("# Deleting database...\n");
            cx.Database.EnsureDeleted();
            cx.Database.EnsureCreated();

            //Sample (https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL/issues/3)
            if (!cx.Author.Any())
            {
                var autores = new List<Author>(){
                    new Author()
                    {
                        FirstName = "Rafael",
                        LastName = "Almeida",
                        Date = DateTime.Now.AddMilliseconds(1),
                        Identification = Guid.NewGuid(),
                        Books = new List<Book>
                        {
                            new Book {  Title=$"Firebird 3.0.2  "},
                            new Book {  Title=$"Firebird 4.0.0  "}
                        }
                    }
                };
                autores.ForEach(v =>
                {
                    cx.Author.Add(v);
                });
                cx.SaveChanges();
            }
            var obj = cx.Author.Find((long)1);
            cx.Author.Remove(obj);
            cx.SaveChanges();

            ////fifty rows
            for (int i = 0; i < 50; i++)
            {
                cx.Author.Add(new Author
                {
                    FirstName = "Rafael",
                    LastName = "Almeida",
                    Date = DateTime.Now.AddMilliseconds(1),
                    Identification = Guid.NewGuid(),
                    Books = new List<Book>
                            {
                                new Book {  Title=$"Firebird 3.0.2 {i}"},
                                new Book {  Title=$"Firebird 4.0.0 {i}"}
                            }
                });
            }
 
            cx.SaveChanges();

            var AuthorsUpdate1 = cx.Author.Find((long)4); 
            Console.WriteLine($"Before *** {AuthorsUpdate1.FirstName}");
            cx.Attach(AuthorsUpdate1);
            AuthorsUpdate1.FirstName = $"Author Modified {Guid.NewGuid()}";
             
            var AuthorsUpdate2 = cx.Author.Find((long)2);
            cx.Attach(AuthorsUpdate2);
            AuthorsUpdate2.FirstName = $"Author Modified {Guid.NewGuid()}";

            var AuthorsUpdate3 = cx.Author.Find((long)3);
            cx.Attach(AuthorsUpdate3);
            AuthorsUpdate3.FirstName = $"Author Modified {Guid.NewGuid()}";

            cx.SaveChanges();
            var y = cx.Author.Include(p => p.Books).Where(p => p.LastName.Trim()!="A").ToList();
            var Authors = cx.Author.AsNoTracking()
                            .Include(p => p.Books).Where(p=>p.AuthorId<10)
                             .ToList();

            Console.WriteLine($"After *** {Authors.First().FirstName}");
            foreach (var item in Authors)
            {
                Console.WriteLine($"Author #->{item.FirstName}  {item.LastName} ");
                Console.WriteLine($"--------------BOOKS----------------");
                foreach (var book in item.Books)
                    Console.WriteLine($"Book: {book.Title}");
                Console.WriteLine($"-----------------------------------");
            }
            Console.ReadKey();
        }
    }
}
