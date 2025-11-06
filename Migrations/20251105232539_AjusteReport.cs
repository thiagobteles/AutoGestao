using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class AjusteReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_padrao",
                table: "report_templates",
                newName: "padrao");

            migrationBuilder.RenameIndex(
                name: "IX_report_templates_tipo_entidade_is_padrao",
                table: "report_templates",
                newName: "IX_report_templates_tipo_entidade_padrao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "padrao",
                table: "report_templates",
                newName: "is_padrao");

            migrationBuilder.RenameIndex(
                name: "IX_report_templates_tipo_entidade_padrao",
                table: "report_templates",
                newName: "IX_report_templates_tipo_entidade_is_padrao");
        }
    }
}
