using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoVirtus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCampoSenha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BloqueadoAte",
                table: "Pessoas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Pessoas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TentativasLogin",
                table: "Pessoas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimoLogin",
                table: "Pessoas",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloqueadoAte",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "TentativasLogin",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "UltimoLogin",
                table: "Pessoas");
        }
    }
}
