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
            for (int i = 1; i <= 500; i++)
            {
                cx.Blog.Add(new Blog
                {
                    Description = $"Blog {i}",
                    Observations = $"Test Add Blog {i}",
                    Date = DateTime.Now

                });
            }
            
            Console.WriteLine($"Registros Inseridos {cx.SaveChanges()}...");
            Console.WriteLine($"--------------------------------------------------------");
             
            var dados = cx.Blog.OrderByDescending(p => p.Id).OrderByDescending(p=>p.Date).Take(10).ToList();
            foreach(var item in dados) 
                Console.WriteLine(item.Id + "\t\t" + item.Description + "\t\t" + item.Observations + "\t\t" + item.Date);


            Console.WriteLine($"--------------------------------------------------------");
            Console.WriteLine($"Atualizando registro  ");
            Console.WriteLine($"--------------------------------------------------------");
            Random rnd = new Random();
            for (int i = 1; i < 20; i++)
            {
                var id = rnd.Next(1, 2000) ;
                var registro = cx.Blog.Find(id);
                if (registro != null)
                {
                    cx.Attach(registro);
                    registro.Description = $"REGISTRO  {id}";
                } 
            } 
            Console.WriteLine($"Registros Atualizados... {cx.SaveChanges()}"); 

            Console.ReadKey();
        }
    }
}
