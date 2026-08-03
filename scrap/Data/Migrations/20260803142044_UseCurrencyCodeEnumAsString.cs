using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gemini.Data.Migrations
{
    /// <inheritdoc />
    public partial class UseCurrencyCodeEnumAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Dollar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Euro");
        }
    }
}
