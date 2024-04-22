﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sankabinis.Data;

#nullable disable

namespace Sankabinis.Migrations
{
    [DbContext(typeof(SankabinisContext))]
    partial class SankabinisContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Sankabinis.Models.Automobilis", b =>
                {
                    b.Property<int>("Id_Automobilis")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id_Automobilis"));

                    b.Property<int>("Fk_Naudotojasid_Naudotojas")
                        .HasColumnType("int");

                    b.Property<int>("Galingumas")
                        .HasColumnType("int");

                    b.Property<int>("Kebulas")
                        .HasColumnType("int");

                    b.Property<int>("Klase")
                        .HasColumnType("int");

                    b.Property<int>("Kuro_tipas")
                        .HasColumnType("int");

                    b.Property<string>("Marke")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Modelis")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Numeris")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime>("Pagaminimo_data")
                        .HasColumnType("datetime2");

                    b.Property<int>("Pavaru_deze")
                        .HasColumnType("int");

                    b.Property<int>("Rida")
                        .HasColumnType("int");

                    b.Property<string>("Spalva")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<double>("Svoris")
                        .HasColumnType("float");

                    b.HasKey("Id_Automobilis");

                    b.ToTable("Automobilis");
                });
#pragma warning restore 612, 618
        }
    }
}
