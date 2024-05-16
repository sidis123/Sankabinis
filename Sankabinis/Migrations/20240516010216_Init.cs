using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sankabinis.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "automobilis",
                columns: table => new
                {
                    Id_Automobilis = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Modelis = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Marke = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Numeris = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Galingumas = table.Column<int>(type: "int", nullable: false),
                    Spalva = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    table.PrimaryKey("PK_automobilis", x => x.Id_Automobilis);
                });

            migrationBuilder.CreateTable(
                name: "miestas",
                columns: table => new
                {
                    Id_Miestas = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pavadinimas = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Koordinates = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_miestas", x => x.Id_Miestas);
                });

            migrationBuilder.CreateTable(
                name: "naudotojas",
                columns: table => new
                {
                    Id_Naudotojas = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Vardas_pavarde = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Slapyvardis = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Slaptazodis = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    El_pastas = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Elo = table.Column<int>(type: "int", nullable: false),
                    Lenktyniu_skaicius = table.Column<int>(type: "int", nullable: false),
                    Svoris = table.Column<double>(type: "float", nullable: false),
                    Gimimo_data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Paskyros_sukurimo_data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Laimėta_lenktyniu = table.Column<int>(type: "int", nullable: false),
                    Pralaimėta_lenktyniu = table.Column<int>(type: "int", nullable: false),
                    Pasitikimo_taskai = table.Column<int>(type: "int", nullable: false),
                    Paskutinio_prisijungimo_data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Suspeduotos_busenos_skaicius = table.Column<int>(type: "int", nullable: false),
                    Lytis = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Patirtis = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Busena = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_naudotojas", x => x.Id_Naudotojas);
                });

            migrationBuilder.CreateTable(
                name: "pasiekimas",
                columns: table => new
                {
                    Id_Pasiekimas = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pavadinimas = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Aprasas = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pasiekimas", x => x.Id_Pasiekimas);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "automobilis");

            migrationBuilder.DropTable(
                name: "miestas");

            migrationBuilder.DropTable(
                name: "naudotojas");

            migrationBuilder.DropTable(
                name: "pasiekimas");
        }
    }
}
