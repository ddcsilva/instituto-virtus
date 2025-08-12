﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstitutoVirtus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCpfToPessoa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cpf",
                table: "Pessoas",
                type: "TEXT",
                maxLength: 11,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pessoas_Cpf",
                table: "Pessoas",
                column: "Cpf",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pessoas_Cpf",
                table: "Pessoas");

            migrationBuilder.DropColumn(
                name: "Cpf",
                table: "Pessoas");
        }
    }
}
