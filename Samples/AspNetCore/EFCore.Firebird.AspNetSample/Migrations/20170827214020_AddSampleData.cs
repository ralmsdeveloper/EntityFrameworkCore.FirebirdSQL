using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SouchProd.EFCore.Firebird.AspNetSample.Migrations
{
    public partial class AddSampleData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Authors (AuthorId, Firstname, Lastname) VALUES (1, 'Stephen', 'King');");
            migrationBuilder.Sql("INSERT INTO Authors (AuthorId, Firstname, Lastname) VALUES (2, 'Isaac', 'Asimov');");
            migrationBuilder.Sql("INSERT INTO Books (BookId, AuthorId, Title) VALUES (1, 1, 'The Shining');");
            migrationBuilder.Sql("INSERT INTO Books (BookId, AuthorId, Title) VALUES (2, 1, 'Black House');");
            migrationBuilder.Sql("INSERT INTO Books (BookId, AuthorId, Title) VALUES (3, 1, 'Cujo');");
            migrationBuilder.Sql("INSERT INTO Books (BookId, AuthorId, Title) VALUES (4, 2, 'Prelude to Foundation');");
            migrationBuilder.Sql("INSERT INTO Books (BookId, AuthorId, Title) VALUES (5, 2, 'Foundation');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Books");
            migrationBuilder.Sql("DELETE FROM Authors;");            
        }
    }
}
