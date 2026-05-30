using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeekendCarSales.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarSales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ModelName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    SoldOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PriceWithoutVat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarSales", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarSales");
        }
    }
}
