using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class LeadAjusteFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cnpj",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "cpf",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "data_nascimento",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "rg",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "telefone",
                table: "leads");

            migrationBuilder.DropColumn(
                name: "tipo_contato",
                table: "leads");

            migrationBuilder.RenameColumn(
                name: "tipo_pessoa",
                table: "leads",
                newName: "tipo_retorno_contato");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "leads",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "celular",
                table: "leads",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "tipo_retorno_contato",
                table: "leads",
                newName: "tipo_pessoa");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "leads",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "celular",
                table: "leads",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "cnpj",
                table: "leads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cpf",
                table: "leads",
                type: "character varying(14)",
                maxLength: 14,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_nascimento",
                table: "leads",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rg",
                table: "leads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "telefone",
                table: "leads",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "tipo_contato",
                table: "leads",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
