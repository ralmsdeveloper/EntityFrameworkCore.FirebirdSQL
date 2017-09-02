using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FirebirdSql.EntityFrameworkCore.Firebird.Test
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
          
            //fifty rows
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

            //four rows
            for (int i = 0; i < 5; i++)
            {
                cx.Book.Add(new Book
                {
                    AuthorId = 1,
                    Title = $"Test Book {i}!"
                });
            }
                

            //Five rows
            for (int i = 0; i < 5; i++)
            {
                cx.Test.Add(new Test
                {
                    Id = Guid.NewGuid(),
                    Description = $"Test Guid {i}!"
                });
            }
           

            //Save all
            cx.SaveChanges();

            var Authors = cx.Author
                            .Include(p => p.Books)
                            .Where(p => p.FirstName.Contains("Rafael") ||
                                        p.FirstName.Contains("Jean")).ToList();


            foreach (var item in Authors)
            {
                Console.WriteLine($"Author #->{item.FirstName} {item.LastName} / Identification: {item.Identification.ToString()}");
                Console.WriteLine($"--------------BOOKS----------------");
                foreach (var book in item.Books)
                    Console.WriteLine($"Book: {book.Title}");
                Console.WriteLine($"-----------------------------------");
            }
            Console.ReadKey();
        }
    }
}
