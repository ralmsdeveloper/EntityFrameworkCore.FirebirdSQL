using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq; 

namespace EFCore.FirebirdSqlSQL.Test
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("# Wait... ");
            var cx = new Context();
            Console.WriteLine("# Deleting database...\n");
            cx.Database.EnsureDeleted();
            cx.Database.EnsureCreated(); 

            cx.Author.Add(new Author
            {
                FirstName = "Rafael",
                LastName = "Almeida",
                Books = new List<Book>
                             {
                                  new Book {  Title="Firebird 3.0.1"},
                                  new Book {  Title="Firebird 3.0.2"}
                             }
            });

            cx.Author.Add(new Author
            {
                FirstName = "Ralms",
                LastName = "Developer",
                Books = new List<Book>
                             {
                                  new Book {  Title="Firebird 4Beta"}
                             }
            });

            //Save
            cx.SaveChanges();

            //Use Include (UPPER)
            var Authors = cx.Author.Include(p => p.Books).Where(p => p.LastName.ToUpper().Contains("L")).ToList();
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
