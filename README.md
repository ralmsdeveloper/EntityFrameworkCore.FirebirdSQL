EntityFrameworkCore.FirebirdSql for Firebird Server
=====================

[![GitHub license](https://img.shields.io/badge/license-GPLv2-blue.svg)](https://raw.githubusercontent.com/ralmsdeveloper/EntityFrameworkCore.FirebirdSQL/master/LICENSE) 

## Example of use

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
