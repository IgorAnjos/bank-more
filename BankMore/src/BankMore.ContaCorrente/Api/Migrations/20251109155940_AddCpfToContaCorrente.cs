using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.ContaCorrente.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCpfToContaCorrente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "contacorrente",
                columns: table => new
                {
                    idcontacorrente = table.Column<string>(type: "TEXT(37)", nullable: false),
                    numero = table.Column<int>(type: "INTEGER(10)", nullable: false),
                    cpf = table.Column<string>(type: "TEXT(200)", nullable: false),
                    nome = table.Column<string>(type: "TEXT(100)", nullable: false),
                    ativo = table.Column<bool>(type: "INTEGER(1)", nullable: false, defaultValue: true),
                    senha = table.Column<string>(type: "TEXT(100)", nullable: false),
                    salt = table.Column<string>(type: "TEXT(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contacorrente", x => x.idcontacorrente);
                });

            migrationBuilder.CreateTable(
                name: "idempotencia",
                columns: table => new
                {
                    chave_idempotencia = table.Column<string>(type: "TEXT(37)", nullable: false),
                    requisicao = table.Column<string>(type: "TEXT(1000)", nullable: false),
                    resultado = table.Column<string>(type: "TEXT(1000)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_idempotencia", x => x.chave_idempotencia);
                });

            migrationBuilder.CreateTable(
                name: "movimento",
                columns: table => new
                {
                    idmovimento = table.Column<string>(type: "TEXT(37)", nullable: false),
                    idcontacorrente = table.Column<string>(type: "TEXT(37)", nullable: false),
                    datamovimento = table.Column<string>(type: "TEXT(25)", nullable: false),
                    tipomovimento = table.Column<char>(type: "TEXT(1)", nullable: false),
                    valor = table.Column<decimal>(type: "REAL", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movimento", x => x.idmovimento);
                    table.ForeignKey(
                        name: "FK_movimento_contacorrente_idcontacorrente",
                        column: x => x.idcontacorrente,
                        principalTable: "contacorrente",
                        principalColumn: "idcontacorrente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contacorrente_cpf",
                table: "contacorrente",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contacorrente_numero",
                table: "contacorrente",
                column: "numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movimento_idcontacorrente",
                table: "movimento",
                column: "idcontacorrente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "idempotencia");

            migrationBuilder.DropTable(
                name: "movimento");

            migrationBuilder.DropTable(
                name: "contacorrente");
        }
    }
}
