using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gemini.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddsTargetCurrencyRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExchangeRates_CurrencyId_Date",
                table: "ExchangeRates");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencyId_TargetCurrencyId_Date",
                table: "ExchangeRates",
                columns: new[] { "CurrencyId", "TargetCurrencyId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_TargetCurrencyId",
                table: "ExchangeRates",
                column: "TargetCurrencyId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExchangeRates_Currencies_TargetCurrencyId",
                table: "ExchangeRates",
                column: "TargetCurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExchangeRates_Currencies_TargetCurrencyId",
                table: "ExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeRates_CurrencyId_TargetCurrencyId_Date",
                table: "ExchangeRates");

            migrationBuilder.DropIndex(
                name: "IX_ExchangeRates_TargetCurrencyId",
                table: "ExchangeRates");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_CurrencyId_Date",
                table: "ExchangeRates",
                columns: new[] { "CurrencyId", "Date" },
                unique: true);
        }
    }
}
