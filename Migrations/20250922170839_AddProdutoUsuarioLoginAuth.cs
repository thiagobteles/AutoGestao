using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class AddProdutoUsuarioLoginAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(2814),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(4684));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(6619),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(8483));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(4899),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(6732));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_fotos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(5894),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(7720));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_documentos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(6257),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(8054));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "tarefas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(8513),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 232, DateTimeKind.Utc).AddTicks(291));

            migrationBuilder.AddColumn<int>(
                name: "responsavel_usuario_id",
                table: "tarefas",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "parcelas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(7075),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(8936));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "fornecedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(3482),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(5447));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(7907),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(9720));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesa_tipos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(8156),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(9970));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "clientes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(2253),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(4072));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "avaliacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(7507),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(9332));

            migrationBuilder.CreateTable(
                name: "produtos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    categoria = table.Column<int>(type: "integer", nullable: false),
                    descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    preco_custo = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    preco_venda = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    estoque_atual = table.Column<int>(type: "integer", nullable: false),
                    estoque_minimo = table.Column<int>(type: "integer", nullable: false),
                    estoque_maximo = table.Column<int>(type: "integer", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 785, DateTimeKind.Utc).AddTicks(3418))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_produtos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    senha_hash = table.Column<string>(type: "text", nullable: false),
                    cpf = table.Column<string>(type: "text", nullable: true),
                    telefone = table.Column<string>(type: "text", nullable: true),
                    perfil = table.Column<int>(type: "integer", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    ultimo_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    observacoes = table.Column<string>(type: "text", nullable: true),
                    confirmar_senha = table.Column<string>(type: "text", nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuarios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "itens_venda",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    venda_id = table.Column<int>(type: "integer", nullable: false),
                    produto_id = table.Column<int>(type: "integer", nullable: false),
                    quantidade = table.Column<int>(type: "integer", nullable: false),
                    valor_unitario = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    percentual_desconto = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    valor_desconto = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    valor_total = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 785, DateTimeKind.Utc).AddTicks(4122))
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_itens_venda", x => x.id);
                    table.ForeignKey(
                        name: "fk_itens_venda_produtos_produto_id",
                        column: x => x.produto_id,
                        principalTable: "produtos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_itens_venda_vendas_venda_id",
                        column: x => x.venda_id,
                        principalTable: "vendas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tarefas_responsavel_usuario_id",
                table: "tarefas",
                column: "responsavel_usuario_id");

            migrationBuilder.CreateIndex(
                name: "ix_itens_venda_produto_id",
                table: "itens_venda",
                column: "produto_id");

            migrationBuilder.CreateIndex(
                name: "IX_itens_venda_venda_id_produto_id",
                table: "itens_venda",
                columns: new[] { "venda_id", "produto_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_produtos_codigo",
                table: "produtos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_produtos_nome",
                table: "produtos",
                column: "nome");

            migrationBuilder.AddForeignKey(
                name: "fk_tarefas_usuarios_responsavel_usuario_id",
                table: "tarefas",
                column: "responsavel_usuario_id",
                principalTable: "usuarios",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tarefas_usuarios_responsavel_usuario_id",
                table: "tarefas");

            migrationBuilder.DropTable(
                name: "itens_venda");

            migrationBuilder.DropTable(
                name: "usuarios");

            migrationBuilder.DropTable(
                name: "produtos");

            migrationBuilder.DropIndex(
                name: "ix_tarefas_responsavel_usuario_id",
                table: "tarefas");

            migrationBuilder.DropColumn(
                name: "responsavel_usuario_id",
                table: "tarefas");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(4684),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(2814));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "vendas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(8483),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(6619));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(6732),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(4899));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_fotos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(7720),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(5894));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "veiculo_documentos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(8054),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(6257));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "tarefas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 232, DateTimeKind.Utc).AddTicks(291),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(8513));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "parcelas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(8936),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(7075));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "fornecedores",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(5447),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(3482));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(9720),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(7907));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "despesa_tipos",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(9970),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(8156));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "clientes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(4072),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(2253));

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_alteracao",
                table: "avaliacoes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2025, 9, 22, 4, 43, 24, 231, DateTimeKind.Utc).AddTicks(9332),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2025, 9, 22, 17, 8, 38, 783, DateTimeKind.Utc).AddTicks(7507));
        }
    }
}
