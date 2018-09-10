// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using EFCore.FirebirdSql.FunctionalTests;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class SqlServerTestStoreFactory : ITestStoreFactory
    {
        public static SqlServerTestStoreFactory Instance { get; } = new SqlServerTestStoreFactory();

        protected SqlServerTestStoreFactory()
        {
        }

        public virtual TestStore Create(string storeName)
            => FirebirdTestStore.Create(storeName);

        public virtual TestStore GetOrCreate(string storeName)
            => FirebirdTestStore.GetOrCreate(storeName);

        public virtual IServiceCollection AddProviderServices(IServiceCollection serviceCollection)
            => serviceCollection.AddEntityFrameworkFirebird()
                .AddSingleton<ILoggerFactory>(new TestSqlLoggerFactory());
    }

    public class SqlServerNorthwindTestStoreFactory : SqlServerTestStoreFactory
    {
        public const string Name = "Northwind";
        public static readonly string NorthwindConnectionString = FirebirdTestStore.CreateConnectionString(Name);
        public new static SqlServerNorthwindTestStoreFactory Instance { get; } = new SqlServerNorthwindTestStoreFactory();

        protected SqlServerNorthwindTestStoreFactory()
        {
        }

        public override TestStore GetOrCreate(string storeName)
            => FirebirdTestStore.GetOrCreate(Name);
    }

    public class NorthwindQueryirebirdFixture<TModelCustomizer> : NorthwindQueryRelationalFixture<TModelCustomizer>
        where TModelCustomizer : IModelCustomizer, new()
    {
        protected override ITestStoreFactory TestStoreFactory => SqlServerNorthwindTestStoreFactory.Instance;

        protected override void OnModelCreating(ModelBuilder modelBuilder, DbContext context)
        {
            base.OnModelCreating(modelBuilder, context);

            modelBuilder.Entity<Customer>()
                .Property(c => c.CustomerID)
                .HasColumnType("char(5)");

            modelBuilder.Entity<Employee>(
                b =>
                {
                    b.Property(c => c.EmployeeID).HasColumnType("int");
                    b.Property(c => c.ReportsTo).HasColumnType("int");
                });

            modelBuilder.Entity<Order>(
                b =>
                {
                    b.Property(o => o.EmployeeID).HasColumnType("int");
                    b.Property(o => o.OrderDate).HasColumnType("timestamp");
                });

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasColumnType("decimal");

            modelBuilder.Entity<Product>(
                b =>
                {
                    b.Property(p => p.UnitPrice).HasColumnType("decimal");
                    b.Property(p => p.UnitsInStock).HasColumnType("int");
                });

            modelBuilder.Entity<MostExpensiveProduct>()
                .Property(p => p.UnitPrice)
                .HasColumnType("decimal");
        }
    }
}
