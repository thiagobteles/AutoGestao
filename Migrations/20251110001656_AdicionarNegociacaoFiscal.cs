using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FGT.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarNegociacaoFiscal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "importacoes_negociacoes_fiscais",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    arquivo_excel = table.Column<string>(type: "text", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_importacoes_negociacoes_fiscais", x => x.id);
                    table.ForeignKey(
                        name: "FK_importacoes_negociacoes_fiscais_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "negociacoes_fiscais",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mes_ano_requerimento = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    ufoptante = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    cpf_cnpj_optante = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    nome_optante = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    numero_conta_negociacao = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tipo_negociacao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    modalidade_negociacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    situacao_negociacao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    qtde_parcelas_concedidas = table.Column<int>(type: "integer", nullable: true),
                    qtde_parcelas_atraso = table.Column<int>(type: "integer", nullable: true),
                    valor_consolidado = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    valor_principal = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    valor_multa = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    valor_juros = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    valor_encargo_legal = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_negociacoes_fiscais", x => x.id);
                    table.ForeignKey(
                        name: "FK_negociacoes_fiscais_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_importacoes_negociacoes_fiscais_empresa_id",
                table: "importacoes_negociacoes_fiscais",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_negociacoes_fiscais_empresa_id",
                table: "negociacoes_fiscais",
                column: "empresa_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "importacoes_negociacoes_fiscais");

            migrationBuilder.DropTable(
                name: "negociacoes_fiscais");
        }
    }
}
