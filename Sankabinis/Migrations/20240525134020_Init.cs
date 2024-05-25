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
                name: "atstumas",
                columns: table => new
                {
                    Id_Atstumas = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityId1 = table.Column<int>(type: "int", nullable: false),
                    CityId2 = table.Column<int>(type: "int", nullable: false),
                    Atstumas = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atstumas", x => x.Id_Atstumas);
                });

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
                name: "lenktynes",
                columns: table => new
                {
                    Id_Lenktynes = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User1Id = table.Column<int>(type: "int", nullable: false),
                    User2Id = table.Column<int>(type: "int", nullable: false),
                    Pirmo_naudotojo_patvirtinimas = table.Column<bool>(type: "bit", nullable: false),
                    Antro_naudotojo_patvirtinimas = table.Column<bool>(type: "bit", nullable: false),
                    Automobilio_klase = table.Column<int>(type: "int", nullable: false),
                    pasiulytas_laikas = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ar_laikas_patvirtintas = table.Column<bool>(type: "bit", nullable: false),
                    ar_lenktynes_pasibaigusios = table.Column<bool>(type: "bit", nullable: false),
                    rezultatas_pagal_pirmaji_naudotoja = table.Column<int>(type: "int", nullable: false),
                    rezultatas_pagal_antraji_naudotoja = table.Column<int>(type: "int", nullable: false),
                    ar_galutinis_rezultatas = table.Column<bool>(type: "bit", nullable: false),
                    TrackId = table.Column<int>(type: "int", nullable: false),
                    ar_gavo_pirmas = table.Column<bool>(type: "bit", nullable: false),
                    ar_gavo_antras = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lenktynes", x => x.Id_Lenktynes);
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
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false)
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
                    Aprasas = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Generic = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pasiekimas", x => x.Id_Pasiekimas);
                });

            migrationBuilder.CreateTable(
                name: "skundas",
                columns: table => new
                {
                    Id_Skundas = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Paaiskinimas = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Sukurimo_Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Uzdarytas = table.Column<bool>(type: "bit", nullable: false),
                    Id_Lenktynes = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skundas", x => x.Id_Skundas);
                });

            migrationBuilder.CreateTable(
                name: "trasa",
                columns: table => new
                {
                    Id_Trasa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    start_coordinates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    finish_coordinates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trasa", x => x.Id_Trasa);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "atstumas");

            migrationBuilder.DropTable(
                name: "automobilis");

            migrationBuilder.DropTable(
                name: "lenktynes");

            migrationBuilder.DropTable(
                name: "miestas");

            migrationBuilder.DropTable(
                name: "naudotojas");

            migrationBuilder.DropTable(
                name: "pasiekimas");

            migrationBuilder.DropTable(
                name: "skundas");

            migrationBuilder.DropTable(
                name: "trasa");
        }
    }
}
