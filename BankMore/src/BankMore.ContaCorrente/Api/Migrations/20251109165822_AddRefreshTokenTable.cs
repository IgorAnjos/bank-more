using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.ContaCorrente.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "refresh_token",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT(37)", nullable: false),
                    id_conta_corrente = table.Column<string>(type: "TEXT(37)", nullable: false),
                    token_hash = table.Column<string>(type: "TEXT(64)", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    data_expiracao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    revogado = table.Column<bool>(type: "INTEGER", nullable: false),
                    data_revogacao = table.Column<DateTime>(type: "TEXT", nullable: true),
                    motivo_revogacao = table.Column<string>(type: "TEXT(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_data_expiracao",
                table: "refresh_token",
                column: "data_expiracao");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_id_conta_corrente",
                table: "refresh_token",
                column: "id_conta_corrente");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_token_hash",
                table: "refresh_token",
                column: "token_hash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "refresh_token");
        }
    }
}
