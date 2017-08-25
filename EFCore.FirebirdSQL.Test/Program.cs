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

            cx.Products.Add(new Product
            {
                Description = "Description Sample",
                Name = "Product Sample Name",
                Update = DateTime.Now,
                Locked = false
            });
            Console.WriteLine($"Registro Inserido: {cx.SaveChanges()}.");


            var RangeProduct = new List<Product>
            {
                new Product {
                                Description = "Description Sample",
                                Name = "Product Sample Name",
                                Update = DateTime.Now,
                                Locked = false
                            },
                new Product {
                                Description = "Description Sample",
                                Name = "Product Sample Name",
                                Update = DateTime.Now,
                                Locked = true
                            }

            };
            cx.Products.AddRange(RangeProduct);
            Console.WriteLine($"Registros Inseridos Range: {cx.SaveChanges()}.");

            Console.WriteLine($"Data Inicio: {DateTime.Now}");

            for (int i = 1; i <= 10; i++)
            {
                cx.Products.Add(new Product
                {
                    Description = $"Description Sample {i}",
                    Name = $"Product Sample Name {i}",
                    Update = DateTime.Now,
                    Locked = false
                });
            }
            Console.WriteLine($"Registros Inseridos For: {cx.SaveChangesAsync().Result}");

            var dados = cx.Products.OrderByDescending(p => p.Id).Take(10).ToList();
            foreach (var item in dados)
                Console.WriteLine(item.Id + "\t\t" + item.Name);

            Console.WriteLine($"--------------------------------------------------------");
            Console.WriteLine($"Atualizando registro  ");
            Console.WriteLine($"--------------------------------------------------------");
            Random rnd = new Random();
            for (int i = 1; i < 10; i++)
            {
                var registro = cx.Products.Find(1);
                if (registro != null)
                {
                    cx.Attach(registro);
                    registro.Name = $"Product Alter {registro.Id}";
                    registro.Price = i * 2.33m;
                }
            }
            Console.WriteLine($"Registros Atualizados... {cx.SaveChanges()}");

            Console.WriteLine($"--------------------------------------------------------");
            Console.WriteLine($"Exluindo registros  ");
            Console.WriteLine($"--------------------------------------------------------");

            for (int i = 1; i < 5; i++)
            {

                var registro = cx.Products.Find(1);
                if (registro != null)
                    cx.Products.Remove(registro);

                registro = cx.Products.Find(2);
                if (registro != null)
                    cx.Products.Remove(registro);

            }
            Console.WriteLine($"Registros Excluidos... {cx.SaveChanges()}");

            Console.ReadKey();
        }
    }
}
