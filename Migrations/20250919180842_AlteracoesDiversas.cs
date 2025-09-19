using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class AlteracoesDiversas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 84, DateTimeKind.Utc).AddTicks(8225),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(2951));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(2300),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(6910));

            migrationBuilder.AlterColumn<int>(
                name: "tipo_veiculo",
                table: "veiculos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "status_veiculo",
                table: "veiculos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "situacao",
                table: "veiculos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "portas",
                table: "veiculos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "placa",
                table: "veiculos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<int>(
                name: "pericia_cautelar",
                table: "veiculos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "motorizacao",
                table: "veiculos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<long>(
                name: "km_saida",
                table: "veiculos",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "especie",
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
                defaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(386),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(5032));

            migrationBuilder.AlterColumn<int>(
                name: "combustivel",
                table: "veiculos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "cambio",
                table: "veiculos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ano_modelo",
                table: "veiculos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ano_fabricacao",
                table: "veiculos",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "origem_veiculo",
                table: "veiculos",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_fotos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(1490),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(6130));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_documentos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(1872),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(6499));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "tarefas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(4405),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 549, DateTimeKind.Utc).AddTicks(735));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "parcelas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(2837),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(7370));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "fornecedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 84, DateTimeKind.Utc).AddTicks(9092),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(3787));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(3767),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(9658));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesa_tipos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(4024),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 549, DateTimeKind.Utc).AddTicks(272));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "clientes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 84, DateTimeKind.Utc).AddTicks(7429),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(2291));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "avaliacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(3297),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(7838));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "origem_veiculo",
                table: "veiculos");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(2951),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 84, DateTimeKind.Utc).AddTicks(8225));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(6910),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(2300));

            migrationBuilder.AlterColumn<int>(
                name: "tipo_veiculo",
                table: "veiculos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "status_veiculo",
                table: "veiculos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "situacao",
                table: "veiculos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "portas",
                table: "veiculos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "placa",
                table: "veiculos",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "pericia_cautelar",
                table: "veiculos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "motorizacao",
                table: "veiculos",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "km_saida",
                table: "veiculos",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "especie",
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
                defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(5032),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(386));

            migrationBuilder.AlterColumn<int>(
                name: "combustivel",
                table: "veiculos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "cambio",
                table: "veiculos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ano_modelo",
                table: "veiculos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ano_fabricacao",
                table: "veiculos",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_fotos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(6130),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(1490));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_documentos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(6499),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(1872));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "tarefas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 549, DateTimeKind.Utc).AddTicks(735),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(4405));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "parcelas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(7370),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(2837));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "fornecedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(3787),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 84, DateTimeKind.Utc).AddTicks(9092));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(9658),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(3767));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesa_tipos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 549, DateTimeKind.Utc).AddTicks(272),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(4024));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "clientes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(2291),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 84, DateTimeKind.Utc).AddTicks(7429));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "avaliacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 18, 4, 12, 21, 548, DateTimeKind.Utc).AddTicks(7838),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 19, 18, 8, 42, 85, DateTimeKind.Utc).AddTicks(3297));
        }
    }
}
