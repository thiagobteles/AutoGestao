using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class Report : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "report_templates",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo_entidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    template_json = table.Column<string>(type: "text", nullable: false),
                    is_padrao = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_report_templates", x => x.id);
                    table.ForeignKey(
                        name: "FK_report_templates_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "report_template_usages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_report_template = table.Column<long>(type: "bigint", nullable: false),
                    tipo_entidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entidade_id = table.Column<long>(type: "bigint", nullable: false),
                    data_geracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    id_usuario = table.Column<long>(type: "bigint", nullable: true),
                    usuario_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
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
                    table.PrimaryKey("pk_report_template_usages", x => x.id);
                    table.ForeignKey(
                        name: "FK_report_template_usages_empresas_empresa_id",
                        column: x => x.empresa_id,
                        principalTable: "empresas",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_report_template_usages_report_templates_id_report_template",
                        column: x => x.id_report_template,
                        principalTable: "report_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_report_template_usages_data_geracao",
                table: "report_template_usages",
                column: "data_geracao");

            migrationBuilder.CreateIndex(
                name: "IX_report_template_usages_empresa_id",
                table: "report_template_usages",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_template_usages_id_report_template",
                table: "report_template_usages",
                column: "id_report_template");

            migrationBuilder.CreateIndex(
                name: "IX_report_template_usages_tipo_entidade_entidade_id",
                table: "report_template_usages",
                columns: new[] { "tipo_entidade", "entidade_id" });

            migrationBuilder.CreateIndex(
                name: "IX_report_templates_empresa_id",
                table: "report_templates",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "IX_report_templates_tipo_entidade",
                table: "report_templates",
                column: "tipo_entidade");

            migrationBuilder.CreateIndex(
                name: "IX_report_templates_tipo_entidade_is_padrao",
                table: "report_templates",
                columns: new[] { "tipo_entidade", "is_padrao" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "report_template_usages");

            migrationBuilder.DropTable(
                name: "report_templates");
        }
    }
}
