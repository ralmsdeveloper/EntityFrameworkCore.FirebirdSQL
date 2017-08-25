using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EFCore.FirebirdSQL.Test.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("FirebirdSql:ValueGenerationStrategy", FirebirdSqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "BLOB SUB_TYPE TEXT", nullable: true),
                    Locked = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    Name = table.Column<string>(type: "VARCHAR(100)", maxLength: 100, nullable: true),
                    Price = table.Column<decimal>(type: "DECIMAL(18,4)", nullable: false),
                    Quant = table.Column<double>(type: "DOUBLE PRECISION", nullable: false),
                    Update = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "Id",
                table: "Products",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
