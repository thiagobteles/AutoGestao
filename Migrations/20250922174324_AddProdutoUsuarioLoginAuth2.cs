using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class AddProdutoUsuarioLoginAuth2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 755, DateTimeKind.Utc).AddTicks(9447),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(2814));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(3275),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(6619));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(1593),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(4899));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_fotos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(2577),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(5894));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_documentos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(2887),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(6257));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "tarefas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(5003),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(8513));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "produtos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 757, DateTimeKind.Utc).AddTicks(8701),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 785, DateTimeKind.Utc).AddTicks(3418));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "parcelas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(3650),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(7075));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "itens_venda",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 757, DateTimeKind.Utc).AddTicks(9329),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 785, DateTimeKind.Utc).AddTicks(4122));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "fornecedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(191),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(3482));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(4496),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(7907));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesa_tipos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(4722),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(8156));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "clientes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 755, DateTimeKind.Utc).AddTicks(8951),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(2253));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "avaliacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(4084),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(7507));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(2814),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 755, DateTimeKind.Utc).AddTicks(9447));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(6619),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(3275));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(4899),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(1593));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_fotos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(5894),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(2577));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_documentos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(6257),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(2887));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "tarefas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(8513),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(5003));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "produtos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 785, DateTimeKind.Utc).AddTicks(3418),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 757, DateTimeKind.Utc).AddTicks(8701));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "parcelas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(7075),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(3650));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "itens_venda",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 785, DateTimeKind.Utc).AddTicks(4122),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 757, DateTimeKind.Utc).AddTicks(9329));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "fornecedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(3482),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(191));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(7907),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(4496));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesa_tipos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(8156),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(4722));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "clientes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(2253),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 755, DateTimeKind.Utc).AddTicks(8951));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "avaliacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(7507),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(4084));
        }
    }
}
