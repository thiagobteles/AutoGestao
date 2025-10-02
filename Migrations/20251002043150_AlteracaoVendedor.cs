using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class AlteracaoVendedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cnpj",
                table: "vendedores",
                type: "character varying(18)",
                maxLength: 18,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_nascimento",
                table: "vendedores",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rg",
                table: "vendedores",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tipo_pessoa",
                table: "vendedores",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cnpj",
                table: "vendedores");

            migrationBuilder.DropColumn(
                name: "data_nascimento",
                table: "vendedores");

            migrationBuilder.DropColumn(
                name: "rg",
                table: "vendedores");

            migrationBuilder.DropColumn(
                name: "tipo_pessoa",
                table: "vendedores");
        }
    }
}
