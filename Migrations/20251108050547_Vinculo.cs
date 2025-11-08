using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class Vinculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "id_empresa_cliente",
                table: "usuarios",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_id_empresa_cliente",
                table: "usuarios",
                column: "id_empresa_cliente",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_usuarios_empresas_clientes_id_empresa_cliente",
                table: "usuarios",
                column: "id_empresa_cliente",
                principalTable: "empresas_clientes",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usuarios_empresas_clientes_id_empresa_cliente",
                table: "usuarios");

            migrationBuilder.DropIndex(
                name: "IX_usuarios_id_empresa_cliente",
                table: "usuarios");

            migrationBuilder.DropColumn(
                name: "id_empresa_cliente",
                table: "usuarios");
        }
    }
}
