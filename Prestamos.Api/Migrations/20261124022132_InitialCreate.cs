using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Prestamos.Api.Migrations;

/// <inheritdoc />
public partial class _20261124022132_InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Cliente",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                NombreCompleto = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                Correo = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                IngresoMensual = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Cliente", x => x.id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Prestamo",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                ClienteId = table.Column<int>(type: "int", nullable: false),
                Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TasaAnual = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                PlazoMeses = table.Column<int>(type: "int", nullable: false),
                FechaInicio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                CuotaMensual = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                TotalAPagar = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Estado = table.Column<string>(type: "varchar(20)", nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Prestamo", x => x.id);
                table.ForeignKey(
                    name: "FK_Prestamo_Cliente_ClienteId",
                    column: x => x.ClienteId,
                    principalTable: "Cliente",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Cuota",
            columns: table => new
            {
                id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                PrestamoId = table.Column<int>(type: "int", nullable: false),
                Numero = table.Column<int>(type: "int", nullable: false),
                FechaVencimiento = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                FechaPago = table.Column<DateTime>(type: "datetime(6)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Cuota", x => x.id);
                table.ForeignKey(
                    name: "FK_Cuota_Prestamo_PrestamoId",
                    column: x => x.PrestamoId,
                    principalTable: "Prestamo",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_Cliente_Correo",
            table: "Cliente",
            column: "Correo",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Cuota_PrestamoId_Numero",
            table: "Cuota",
            columns: new[] { "PrestamoId", "Numero" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Prestamo_ClienteId",
            table: "Prestamo",
            column: "ClienteId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Cuota");

        migrationBuilder.DropTable(
            name: "Prestamo");

        migrationBuilder.DropTable(
            name: "Cliente");
    }
}
