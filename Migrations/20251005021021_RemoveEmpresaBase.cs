using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmpresaBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "empresa_id",
                table: "empresas",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "id_empresa",
                table: "empresas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_empresas_empresa_id",
                table: "empresas",
                column: "empresa_id");

            migrationBuilder.AddForeignKey(
                name: "FK_empresas_empresas_empresa_id",
                table: "empresas",
                column: "empresa_id",
                principalTable: "empresas",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_empresas_empresas_empresa_id",
                table: "empresas");

            migrationBuilder.DropIndex(
                name: "IX_empresas_empresa_id",
                table: "empresas");

            migrationBuilder.DropColumn(
                name: "empresa_id",
                table: "empresas");

            migrationBuilder.DropColumn(
                name: "id_empresa",
                table: "empresas");
        }
    }
}
