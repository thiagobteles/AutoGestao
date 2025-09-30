using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class Ajuste2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "vendas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "veiculos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "veiculo_marcas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "veiculo_marca_modelos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "veiculo_localizacoes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "veiculo_fotos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "veiculo_filiais",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "veiculo_documentos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "veiculo_cores",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "tarefas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "parcelas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "despesas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "despesa_tipos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "avaliacoes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                table: "audit_logs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ativo",
                table: "vendas");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "veiculos");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "veiculo_marcas");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "veiculo_marca_modelos");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "veiculo_localizacoes");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "veiculo_fotos");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "veiculo_filiais");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "veiculo_documentos");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "veiculo_cores");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "tarefas");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "parcelas");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "despesas");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "despesa_tipos");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "avaliacoes");

            migrationBuilder.DropColumn(
                name: "ativo",
                table: "audit_logs");
        }
    }
}
