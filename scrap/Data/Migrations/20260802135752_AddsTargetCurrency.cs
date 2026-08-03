using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace gemini.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddsTargetCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TargetCurrencyId",
                table: "ExchangeRates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Code", "Name" },
                values: new object[,]
                {
                    { 3, "EUR", "Euro" },
                    { 4, "USD", "Euro" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "TargetCurrencyId",
                table: "ExchangeRates");
        }
    }
}
