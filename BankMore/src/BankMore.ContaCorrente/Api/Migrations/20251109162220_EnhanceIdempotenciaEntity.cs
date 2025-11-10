using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.ContaCorrente.Api.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceIdempotenciaEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "data_criacao",
                table: "idempotencia",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "data_expiracao",
                table: "idempotencia",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "metadata",
                table: "idempotencia",
                type: "TEXT(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "resultado_hash",
                table: "idempotencia",
                type: "TEXT(64)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "idempotencia",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "data_criacao",
                table: "idempotencia");

            migrationBuilder.DropColumn(
                name: "data_expiracao",
                table: "idempotencia");

            migrationBuilder.DropColumn(
                name: "metadata",
                table: "idempotencia");

            migrationBuilder.DropColumn(
                name: "resultado_hash",
                table: "idempotencia");

            migrationBuilder.DropColumn(
                name: "status",
                table: "idempotencia");
        }
    }
}
