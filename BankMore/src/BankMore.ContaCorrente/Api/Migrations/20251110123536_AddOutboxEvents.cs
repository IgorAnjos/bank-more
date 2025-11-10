using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.ContaCorrente.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outbox_events",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT(37)", nullable: false),
                    topic = table.Column<string>(type: "TEXT(100)", nullable: false),
                    event_type = table.Column<string>(type: "TEXT(100)", nullable: false),
                    payload = table.Column<string>(type: "TEXT", nullable: false),
                    partition_key = table.Column<string>(type: "TEXT(100)", nullable: true),
                    created_at = table.Column<DateTime>(type: "TEXT", nullable: false),
                    processed_at = table.Column<DateTime>(type: "TEXT", nullable: true),
                    processed = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    retry_count = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    error_message = table.Column<string>(type: "TEXT(500)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_events", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_outbox_events_processed_created_at",
                table: "outbox_events",
                columns: new[] { "processed", "created_at" });

            migrationBuilder.CreateIndex(
                name: "IX_outbox_events_topic",
                table: "outbox_events",
                column: "topic");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_events");
        }
    }
}
