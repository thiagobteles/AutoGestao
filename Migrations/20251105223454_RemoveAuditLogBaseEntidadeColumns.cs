using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAuditLogBaseEntidadeColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "alterado_por_usuario_id",
                table: "audit_logs");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "audit_logs");

            migrationBuilder.DropColumn(
                name: "criado_por_usuario_id",
                table: "audit_logs");

            migrationBuilder.DropColumn(
                name: "data_cadastro",
                table: "audit_logs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "alterado_por_usuario_id",
                table: "audit_logs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "audit_logs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "criado_por_usuario_id",
                table: "audit_logs",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_cadastro",
                table: "audit_logs",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
