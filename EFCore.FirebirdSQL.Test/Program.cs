using FirebirdSql.Data.FirebirdClient;
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
            var con = new FbConnection();
            var strConnection = "User=SYSDBA;Password=masterkey;Database=R:\\RalmsDev.fdb;DataSource=localhost;Port=2017;Dialect=3;Charset=UTF8;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=100;Packet Size=8192;ServerType=0;";
            var csb = new FbConnectionStringBuilder(strConnection);
            var dbName = csb.Database; // "" + csb.Database.Replace('', ' ') + "";
            //csb.Database = "";
            using (var connection = new FbConnection(csb.ConnectionString))
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = $"CREATE DATABASE {dbName} CHARACTER SET utf8 COLLATE utf8_unicode_ci";
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("# Wait... ");
            var cx = new Context();
            Console.WriteLine("# Deleting database...\n");
            cx.Database.EnsureDeleted();
            cx.Database.EnsureCreated();
            //Add Pool
            for (int i = 0; i < 1000; i++)
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
            for (int i = 0; i < 2000; i++)
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
