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
            Console.WriteLine("# Wait... ");
            var cx = new Context(); 
            Console.WriteLine("# Deleting database...");
            cx.Database.EnsureDeleted();
            cx.Database.EnsureCreated();

            cx.Blog.Add(new Blog
            {
                Url = "https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL"
            });
            Console.WriteLine($"Registro Inserido: {cx.SaveChanges()}.");


            var RangeBlog = new List<Blog>
            {
                new Blog{ Url="https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL"  },
                new Blog{ Url="https://github.com/ralmsdeveloper/"  },
                new Blog{ Url="https://blog.ralms.net"  },
                new Blog{ Url="https://ralms.net"  } 
            };
            cx.Blog.AddRange(RangeBlog);
            Console.WriteLine($"Registros Inseridos Range: {cx.SaveChanges()}.");

            for (int i = 1; i <= 20; i++)
            {
                cx.Blog.Add(new Blog
                {
                    Url = "https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL",
                    Autor = $"Ralms {i}"
                });
            }
            Console.WriteLine($"Registros Inseridos For: {cx.SaveChanges()}");


            var dados = cx.Blog.OrderByDescending(p => p.BlogId).Take(10).ToList();
            foreach (var item in dados)
                Console.WriteLine(item.BlogId + "\t\t" + item.Url);

            Console.WriteLine($"--------------------------------------------------------");
            Console.WriteLine($"Atualizando registro  ");
            Console.WriteLine($"--------------------------------------------------------");
            Random rnd = new Random();
            for (int i = 1; i < 30; i++)
            {
                var id = rnd.Next(1, 30);
                var registro = cx.Blog.Find(id);
                if (registro != null)
                {
                    cx.Attach(registro);
                    registro.Url = "www.ralms.net";
                    registro.Autor = $"Ralms {id}";
                }
            }
            Console.WriteLine($"Registros Atualizados... {cx.SaveChanges()}");


            Console.WriteLine($"--------------------------------------------------------");
            Console.WriteLine($"Exluindo registros  ");
            Console.WriteLine($"--------------------------------------------------------");

            for (int i = 1; i < 5; i++)
            {

                var registro = cx.Blog.Find(1);
                if (registro != null)
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
