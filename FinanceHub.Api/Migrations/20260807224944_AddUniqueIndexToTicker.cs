using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToTicker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Ativos_Ticker",
                table: "Ativos",
                column: "Ticker",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ativos_Ticker",
                table: "Ativos");
        }
    }
}
