using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.FirebirdSqlSQL.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*** INITIALIZING ****");
            var cx = new Context();

            Console.WriteLine("Deletando banco...");
            cx.Database.EnsureDeleted();
            Console.WriteLine("Inserindo registros...");
            cx.Database.EnsureCreated();
            for (int i = 1; i <= 10; i++)
            {
                cx.Blog.Add(new Blog
                {
                     Url= "https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL"  
                });
            }
            
            Console.WriteLine($"Registros Inseridos {cx.SaveChanges()}...");
            Console.WriteLine($"--------------------------------------------------------");

            var dados = cx.Blog.OrderByDescending(p => p.BlogId).Take(10).ToList();
            foreach (var item in dados)
                Console.WriteLine(item.BlogId + "\t\t" + item.Url);


            Console.WriteLine($"--------------------------------------------------------");
            Console.WriteLine($"Atualizando registro  ");
            Console.WriteLine($"--------------------------------------------------------");
            Random rnd = new Random();
            for (int i = 1; i < 20; i++)
            {
                var id = rnd.Next(1, 20);
                var registro = cx.Blog.Find(id);
                if (registro != null)
                {
                    cx.Attach(registro);
                    registro.Url = "www.ralms.net";
                }
            }
            Console.WriteLine($"Registros Atualizados... {cx.SaveChanges()}");


            Console.WriteLine($"--------------------------------------------------------");
            Console.WriteLine($"Exluindo registros  ");
            Console.WriteLine($"--------------------------------------------------------");
           
            for (int i = 1; i < 5; i++)
            {
          
                var registro = cx.Blog.Find(1);
                if(registro != null)
                    cx.Blog.Remove(registro);

                registro = cx.Blog.Find(2);
                if (registro != null)
                    cx.Blog.Remove(registro);

            }
            Console.WriteLine($"Registros Excluidos... {cx.SaveChanges()}");


            Console.ReadKey();
        }
    }
}
