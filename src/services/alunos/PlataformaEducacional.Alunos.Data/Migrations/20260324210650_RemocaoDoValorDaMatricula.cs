using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaEducacional.Alunos.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemocaoDoValorDaMatricula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PedidoId",
                table: "Matriculas");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "Matriculas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PedidoId",
                table: "Matriculas",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "Valor",
                table: "Matriculas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
