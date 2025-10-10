using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTemplateUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "report_template_usages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "report_template_usages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empresa_id = table.Column<long>(type: "bigint", nullable: true),
                    id_report_template = table.Column<long>(type: "bigint", nullable: false),
                    alterado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criado_por_usuario_id = table.Column<long>(type: "bigint", nullable: true),
                    data_alteracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    data_geracao = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    entidade_id = table.Column<long>(type: "bigint", nullable: false),
                    id_empresa = table.Column<long>(type: "bigint", nullable: false),
                    id_usuario = table.Column<long>(type: "bigint", nullable: true),
                    tipo_entidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    usuario_nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
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
                columns: ["tipo_entidade", "entidade_id"]);
        }
    }
}
