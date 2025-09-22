using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class NovoFormato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(4684),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(5062));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(8483),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(8854));

            migrationBuilder.AlterColumn<int>(
                name: "veiculo_marca_modelo_id",
                table: "veiculos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "veiculo_marca_id",
                table: "veiculos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(6732),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(7110));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_fotos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(7720),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(8081));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_documentos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(8054),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(8418));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "tarefas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 232, DateTimeKind.Utc).AddTicks(291),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 270, DateTimeKind.Utc).AddTicks(703));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "parcelas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(8936),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(9341));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "fornecedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(5447),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(5858));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(9720),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 270, DateTimeKind.Utc).AddTicks(138));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesa_tipos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(9970),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 270, DateTimeKind.Utc).AddTicks(382));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "clientes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(4072),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(4298));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "avaliacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(9332),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(9748));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(5062),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(4684));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(8854),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(8483));

            migrationBuilder.AlterColumn<int>(
                name: "veiculo_marca_modelo_id",
                table: "veiculos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "veiculo_marca_id",
                table: "veiculos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(7110),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(6732));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_fotos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(8081),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(7720));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_documentos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(8418),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(8054));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "tarefas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 270, DateTimeKind.Utc).AddTicks(703),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 232, DateTimeKind.Utc).AddTicks(291));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "parcelas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(9341),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(8936));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "fornecedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(5858),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(5447));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 270, DateTimeKind.Utc).AddTicks(138),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(9720));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesa_tipos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 270, DateTimeKind.Utc).AddTicks(382),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(9970));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "clientes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(4298),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(4072));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "avaliacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 20, 4, 20, 28, 269, DateTimeKind.Utc).AddTicks(9748),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(9332));
        }
    }
}
