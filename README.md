## Temporary repository

**Attention! This repository will be merged with the [FirebirdSql](https://github.com/cincuranet/FirebirdSql.Data.FirebirdClient/) repository, intent is to unite all our efforts.**
**The same will be officially integrated with FirebirdSQL in a few days!**

I want to thank Jiri Cincura for helping me see the growth of the project. 
For striving to help by analyzing every part of the code!

**Atenção! Este repositório será mesclado com o repositório [FirebirdSql](https://github.com/cincuranet/FirebirdSql.Data.FirebirdClient/), a intenção é unir todos nossos esforços.**
**O mesmo será integrado de forma Oficial com o FirebirdSQL em alguns dias!**

Quero agradecer a Jiri Cincura por me ajudar a ver o crescimento do projeto. 
Por se esforçar para ajudar, analisando cada parte do código!


EntityFrameworkCore.FirebirdSql for Firebird Server
=====================
[![label](https://img.shields.io/github/issues-raw/badges/shields/website.svg?style=plastic)](https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSql)
[![GitHub license](https://img.shields.io/badge/license-GPLv2-blue.svg)](https://raw.githubusercontent.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSql/master/LICENSE) [![Travis build status](https://img.shields.io/travis/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL.svg?label=travis-ci&branch=dev&style=plastic)](https://travis-ci.org/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL/branches)
[![AppVeyor build status](https://img.shields.io/appveyor/ci/ralmsdeveloper/EntityFrameworkCore-FirebirdSQL/master.svg?label=appveyor&style=plastic)](https://ci.appveyor.com/project/ralmsdeveloper/entityframeworkcore-firebirdsql/branch/master)  



Provider               | Package name                              | Stable (`master` branch)    | On test (`dev` branch)
-----------------------|-------------------------------------------|-----------------------------|-------------------------
Firebird SQL           | `EntityFrameworkCore.FirebirdSql` | [![NuGet](https://img.shields.io/nuget/v/EntityFrameworkCore.FirebirdSql.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/EntityFrameworkCore.FirebirdSql/) |  [![NuGet](https://img.shields.io/nuget/v/EntityFrameworkCore.FirebirdSql.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/EntityFrameworkCore.FirebirdSql/)


The EntityFrameworkCore.FirebirdSql is an Entity Framework Core access provider for Firebird SQL, compatible with version 3.X and earlier versions 2.x.

Same uses the ADO.NET Library  [Firebird Client](https://github.com/cincuranet/FirebirdSql.Data.FirebirdClient)  written by friend Cincura.


##  What we already have:
All basic operations are working well

Insert :heavy_check_mark: Update  :heavy_check_mark: Delete :heavy_check_mark:

Insert Bulk :heavy_check_mark: Update Bulk :heavy_check_mark: Delete Bulk :heavy_check_mark:

Includes :heavy_check_mark: Complex Querys :heavy_check_mark: 

##  Supports: 
Guid, TimeStamp, Date, BigInt, Varchar, Text

IDENTITY INCREMENT FOR FIREBIRD 3.X And 4.0 (Alpha)




## Example of use DBContext

 ```csharp
 //DataContext
 public class BlogContext : DbContext
 {
        
        public DbSet<Blog> Blog { get; set; }
        public DbSet<Post> Posts { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { 
            string connectionString = "...";
            optionsBuilder.UseFirebirdSql(connectionString);   
        }
        protected override void OnModelCreating(ModelBuilder modelo)
        {
            //Fluent Api
            modelo.Entity<Blog>(entity =>
            {
                entity.HasIndex(e => e.BlogId)
                    .HasName("Id")
                    .IsUnique();
            });
        }
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; } 
        public List<Post> Posts { get; set; }
    }
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }
 }     
```

## Example of use add
```csharp
//Sample Use
 var cx = new BlogContext();  
 
 //one
 cx.Blog.Add(new Blog
 {
     Url = "https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSql"
 });
 cx.SaveChanges();
 
 //Range
 var RangeBlog = new List<Blog>
 {
      new Blog{ Url="https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSql"  },
      new Blog{ Url="https://github.com/ralmsdeveloper/"  },
      new Blog{ Url="https://blog.ralms.net"  },
      new Blog{ Url="https://ralms.net"  } 
 };
 cx.Blog.AddRange(RangeBlog);
 cx.SaveChanges();
```

## Example of use update
```csharp
//Sample Use
 var cx = new BlogContext();  
  
 var blog = cx.Blog.Find(1);
 cx.Attach(registro);
 blog.Url = "www.ralms.net";
 cx.SaveChanges(); 
```

## Example of use delete
```csharp
//Sample Use
 var cx = new BlogContext();  
  
 var blog = cx.Blog.Find(1);
 cx.Delete(blog); 
 cx.SaveChanges(); 
```
## Example of use where
```csharp
//Sample Use
 var cx = new BlogContext();  
  
 var blog = cx.Blog.Where(p => p.BlogId == 1).ToList(); 
```
