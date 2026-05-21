using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinanceService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "currency",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    rate = table.Column<decimal>(type: "numeric(18,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currency", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_favorites",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_favorites", x => new { x.user_id, x.currency_id });
                    table.ForeignKey(
                        name: "FK_user_favorites_currency_currency_id",
                        column: x => x.currency_id,
                        principalTable: "currency",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "currency",
                columns: new[] { "id", "code", "name", "rate" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "USD", "US Dollar", 90m },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "EUR", "Euro", 98m },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "RUB", "Russian Ruble", 1m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_currency_code",
                table: "currency",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_favorites_currency_id",
                table: "user_favorites",
                column: "currency_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_favorites");

            migrationBuilder.DropTable(
                name: "currency");
        }
    }
}
