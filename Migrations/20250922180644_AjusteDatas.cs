using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class AjusteDatas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "confirmar_senha",
                table: "usuarios");

            migrationBuilder.RenameIndex(
                name: "IX_produtos_nome",
                table: "produtos",
                newName: "ix_produto_nome");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(6195),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 755, DateTimeKind.Utc).AddTicks(9447));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(9676),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(3275));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(8070),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(1593));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_fotos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(8987),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(2577));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_documentos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(9306),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(2887));

            migrationBuilder.AlterColumn<string>(
                name: "telefone",
                table: "usuarios",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "senha_hash",
                table: "usuarios",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "observacoes",
                table: "usuarios",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nome",
                table: "usuarios",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "usuarios",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "usuarios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(2924),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "cpf",
                table: "usuarios",
                type: "character varying(14)",
                maxLength: 14,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ativo",
                table: "usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "tarefas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(1407),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(5003));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "produtos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(1942),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 757, DateTimeKind.Utc).AddTicks(8701));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "parcelas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(108),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(3650));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "itens_venda",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(2452),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 757, DateTimeKind.Utc).AddTicks(9329));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "fornecedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(6873),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(191));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(867),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(4496));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesa_tipos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(1080),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(4722));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "clientes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(5667),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 755, DateTimeKind.Utc).AddTicks(8951));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "avaliacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(472),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(4084));

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_cpf",
                table: "usuarios",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_email",
                table: "usuarios",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_usuarios_cpf",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_email",
                table: "usuarios");

            migrationBuilder.RenameIndex(
                name: "ix_produto_nome",
                table: "produtos",
                newName: "IX_produtos_nome");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 755, DateTimeKind.Utc).AddTicks(9447),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(6195));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(3275),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(9676));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(1593),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(8070));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_fotos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(2577),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(8987));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_documentos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(2887),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(9306));

            migrationBuilder.AlterColumn<string>(
                name: "telefone",
                table: "usuarios",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "senha_hash",
                table: "usuarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "observacoes",
                table: "usuarios",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "nome",
                table: "usuarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "usuarios",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "usuarios",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(2924));

            migrationBuilder.AlterColumn<string>(
                name: "cpf",
                table: "usuarios",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(14)",
                oldMaxLength: 14,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ativo",
                table: "usuarios",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "confirmar_senha",
                table: "usuarios",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "tarefas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(5003),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(1407));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "produtos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 757, DateTimeKind.Utc).AddTicks(8701),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(1942));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "parcelas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(3650),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(108));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "itens_venda",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 757, DateTimeKind.Utc).AddTicks(9329),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(2452));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "fornecedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(191),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(6873));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(4496),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(867));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesa_tipos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(4722),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(1080));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "clientes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 755, DateTimeKind.Utc).AddTicks(8951),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 425, DateTimeKind.Utc).AddTicks(5667));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "avaliacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 43, 23, 756, DateTimeKind.Utc).AddTicks(4084),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 18, 6, 44, 426, DateTimeKind.Utc).AddTicks(472));
        }
    }
}
