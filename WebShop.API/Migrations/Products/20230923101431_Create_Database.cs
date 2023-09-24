using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebShop.API.Migrations.Products
{
    /// <inheritdoc />
    public partial class Create_Database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "products");

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "products",
                table: "Products",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("12c7f944-5c45-4c2e-805a-4c4bf4615c7a"), "Product #2", 200m },
                    { new Guid("55336abd-482d-4fca-800c-29d9ac471725"), "Product #5", 500m },
                    { new Guid("9406f5a6-0d6a-4708-8b64-aff6bb957ddb"), "Product #3", 300m },
                    { new Guid("b9bb5000-77f0-402a-b02f-d1c7cb9c304e"), "Product #1", 100m },
                    { new Guid("d32e75ee-32fc-4ea9-95b3-e41f3ccf7d2e"), "Product #4", 400m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products",
                schema: "products");
        }
    }
}
