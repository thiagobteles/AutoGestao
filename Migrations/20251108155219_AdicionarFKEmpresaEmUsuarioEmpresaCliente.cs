using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGT.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarFKEmpresaEmUsuarioEmpresaCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_usuario_empresa_cliente_id_empresa",
                table: "usuario_empresa_cliente",
                column: "id_empresa");

            migrationBuilder.AddForeignKey(
                name: "FK_usuario_empresa_cliente_empresas_id_empresa",
                table: "usuario_empresa_cliente",
                column: "id_empresa",
                principalTable: "empresas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_usuario_empresa_cliente_empresas_id_empresa",
                table: "usuario_empresa_cliente");

            migrationBuilder.DropIndex(
                name: "IX_usuario_empresa_cliente_id_empresa",
                table: "usuario_empresa_cliente");
        }
    }
}
