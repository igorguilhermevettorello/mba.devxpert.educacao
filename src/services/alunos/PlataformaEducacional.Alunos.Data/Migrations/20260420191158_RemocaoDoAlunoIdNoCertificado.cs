using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlataformaEducacional.Alunos.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemocaoDoAlunoIdNoCertificado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificados_Alunos_AlunoId",
                table: "Certificados");

            migrationBuilder.DropIndex(
                name: "IX_Certificados_AlunoId",
                table: "Certificados");

            migrationBuilder.DropColumn(
                name: "AlunoId",
                table: "Certificados");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AlunoId",
                table: "Certificados",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Certificados_AlunoId",
                table: "Certificados",
                column: "AlunoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificados_Alunos_AlunoId",
                table: "Certificados",
                column: "AlunoId",
                principalTable: "Alunos",
                principalColumn: "Id");
        }
    }
}
