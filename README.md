EntityFrameworkCore.FirebirdSql for Firebird Server
=====================
[![label](https://img.shields.io/github/issues-raw/badges/shields/website.svg?style=plastic)](https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL)
[![GitHub license](https://img.shields.io/badge/license-GPLv2-blue.svg)](https://raw.githubusercontent.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL/master/LICENSE) 
[![Nuget count](http://img.shields.io/nuget/v/EntityFrameworkCore.FirebirdSQL.svg)](https://www.nuget.org/packages/EntityFrameworkCore.FirebirdSQL/)


The EntityFrameworkCore.FirebirdSQL is an Entity Framework Core access provider for Firebird SQL, compatible with version 3.X and earlier versions 2.x.

Same uses the ADO.NET Library  [Firebird Client](https://github.com/cincuranet/FirebirdSql.Data.FirebirdClient)  written by friend Cincura.

##  Prediction Release 2.0 - September 05, 2017

##  What we already have:
All basic operations are working well

Insert :heavy_check_mark:, Update  :heavy_check_mark:, Delete :heavy_check_mark:

Insert Bulk :heavy_check_mark:, Update Bulk :heavy_check_mark:, Delete Bulk :heavy_check_mark:

##  Supports: 
Guid, TimeStamp, Date, BigInt, Varchar, Text

IDENTITY INCREMENT FOR FIREBIRD 3.X


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
     Url = "https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL"
 });
 cx.SaveChanges();
 
 //Range
 var RangeBlog = new List<Blog>
 {
      new Blog{ Url="https://github.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL"  },
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
