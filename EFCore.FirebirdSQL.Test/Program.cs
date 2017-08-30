using FirebirdSql.Data.FirebirdClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFCore.FbSQL.Test
{
    class Program
    {

        static void Main(string[] args)
        { 
            //Command Sample Scaffolding
            //Scaffold-DbContext "User=SYSDBA;Password=masterkey;Database=R:\RALMS.FDB;DataSource=127.0.0.1;Port=2017;Dialect=3;Charset=UTF8;Role=;Connection lifetime=15;Pooling=true;Packet Size=8192;ServerType=0;" EntityFrameworkCore.FirebirdSQL -OutputDir Models -Context "FirebirdDbContext" -DataAnnotations -force -verbose
            Console.WriteLine("# Wait... ");
            var cx = new Context();
            Console.WriteLine("# Deleting database...\n");
            cx.Database.EnsureDeleted();
            cx.Database.EnsureCreated();
            //Add Pool
            for (int i = 0; i < 10; i++)
            {
                cx.Author.Add(new Author
                {
                    FirstName = "Rafael",
                    LastName = "Almeida",
                    Date = DateTime.Now.AddMilliseconds(1),
                    Books = new List<Book>
                             {
                                  new Book {  Title="Firebird 3.0.1"},
                                  new Book {  Title="Firebird 3.0.2"}
                             }
                });
            }
            //Add Pool 
            for (int i = 0; i < 10; i++)
            {
                cx.Book.Add(new Book
                {
                    AuthorId = 1,
                    Title = "TESTE"
                });
            }
               
            //Save all
            cx.SaveChanges();  

            //Use Include (UPPER)
            //var Authors = cx.Author.Include(p => p.Books).Where(p => p.LastName.ToUpper().Contains("L")).ToList();
            var Authors = cx.Author.Include(p => p.Books).Where(p => p.Date >= DateTime.Parse("2017-01-01") && p.Date <= DateTime.Now).ToList();

            Console.WriteLine($"-----------------------------------");
            Console.WriteLine("             Author                 ");
            Console.WriteLine($"-----------------------------------");
            foreach (var item in Authors)
            {
                Console.WriteLine($"  #->{item.FirstName} {item.LastName}");
                Console.WriteLine($"--------------BOOKS----------------");
                foreach (var book in item.Books)
                    Console.WriteLine($"{book.Title}");
            }
            Console.ReadKey();
        }
    }
}
