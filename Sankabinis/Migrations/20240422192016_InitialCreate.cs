using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sankabinis.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Automobilis",
                columns: table => new
                {
                    Id_Automobilis = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Modelis = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Marke = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Numeris = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Galingumas = table.Column<int>(type: "int", nullable: false),
                    Spalva = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rida = table.Column<int>(type: "int", nullable: false),
                    Pagaminimo_data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Svoris = table.Column<double>(type: "float", nullable: false),
                    Kuro_tipas = table.Column<int>(type: "int", nullable: false),
                    Pavaru_deze = table.Column<int>(type: "int", nullable: false),
                    Kebulas = table.Column<int>(type: "int", nullable: false),
                    Klase = table.Column<int>(type: "int", nullable: false),
                    Fk_Naudotojasid_Naudotojas = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Automobilis", x => x.Id_Automobilis);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Automobilis");
        }
    }
}
