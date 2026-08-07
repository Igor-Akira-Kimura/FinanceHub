using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAtivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ativo_Bolsas_BolsaId",
                table: "Ativo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ativo",
                table: "Ativo");

            migrationBuilder.RenameTable(
                name: "Ativo",
                newName: "Ativos");

            migrationBuilder.RenameIndex(
                name: "IX_Ativo_BolsaId",
                table: "Ativos",
                newName: "IX_Ativos_BolsaId");

            migrationBuilder.AlterColumn<string>(
                name: "Ticker",
                table: "Ativos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Ativos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataAtualizacao",
                table: "Ativos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Ativos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ativos",
                table: "Ativos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ativos_Bolsas_BolsaId",
                table: "Ativos",
                column: "BolsaId",
                principalTable: "Bolsas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ativos_Bolsas_BolsaId",
                table: "Ativos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ativos",
                table: "Ativos");

            migrationBuilder.DropColumn(
                name: "DataAtualizacao",
                table: "Ativos");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Ativos");

            migrationBuilder.RenameTable(
                name: "Ativos",
                newName: "Ativo");

            migrationBuilder.RenameIndex(
                name: "IX_Ativos_BolsaId",
                table: "Ativo",
                newName: "IX_Ativo_BolsaId");

            migrationBuilder.AlterColumn<string>(
                name: "Ticker",
                table: "Ativo",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Ativo",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ativo",
                table: "Ativo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ativo_Bolsas_BolsaId",
                table: "Ativo",
                column: "BolsaId",
                principalTable: "Bolsas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
