using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdempotencyKeyToCompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "Compras",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE Compras
                SET IdempotencyKey = CONVERT(nvarchar(450), NEWID())
                WHERE IdempotencyKey IS NULL;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "IdempotencyKey",
                table: "Compras",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "UX_Compras_IdempotencyKey",
                table: "Compras",
                column: "IdempotencyKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_Compras_IdempotencyKey",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "Compras");
        }
    }
}
