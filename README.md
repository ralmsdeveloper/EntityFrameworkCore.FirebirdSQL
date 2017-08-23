# EntityFrameworkCore.FirebirdSQL
FirebirdSQL database provider for Entity Framework Core.
=====================

FirebirdSQL Access Provider Using EntityFrameworkCore

## Example of use

 ```csharp
 public class Context : DbContext
    {
        
        public DbSet<Blog> Blog { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { 

            string connectionString =
            "User=SYSDBA;" +
            "Password=masterkey;" +
            $"Database={AppContext.BaseDirectory}Rafael.fdb;" +
            "DataSource=localhost;" +
            "Port=2017;" +
            "Dialect=3;" +
            "Charset=NONE;" +
            "Role=;" +
            "Connection lifetime=15;" +
            "Pooling=true;" +
            "MinPoolSize=1;" +
            "MaxPoolSize=50;" +
            "Packet Size=8192;" +
            "ServerType=0"; 

        }
        protected override void OnModelCreating(ModelBuilder modelo)
        {
            //Fluent Api
            modelo.Entity<Blog>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("Id")
                    .IsUnique();
            });
        }
    }


    public partial class Blog
    {
        [Key]
        public int Id { get; set; }
        public int Access { get; set; }
        [StringLength(20)]
        public string Description { get; set; }
        [StringLength(500)]
        public string Observations { get; set; }
        public DateTime Date { get; set; }

    } 
    
```
