using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class AjusteTeste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "VeiculoMarcaId",
                table: "veiculo_marca_modelos",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_veiculo_marca_modelos_VeiculoMarcaId",
                table: "veiculo_marca_modelos",
                column: "VeiculoMarcaId");

            migrationBuilder.AddForeignKey(
                name: "FK_veiculo_marca_modelos_veiculo_marcas_VeiculoMarcaId",
                table: "veiculo_marca_modelos",
                column: "VeiculoMarcaId",
                principalTable: "veiculo_marcas",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_veiculo_marca_modelos_veiculo_marcas_VeiculoMarcaId",
                table: "veiculo_marca_modelos");

            migrationBuilder.DropIndex(
                name: "IX_veiculo_marca_modelos_VeiculoMarcaId",
                table: "veiculo_marca_modelos");

            migrationBuilder.DropColumn(
                name: "VeiculoMarcaId",
                table: "veiculo_marca_modelos");
        }
    }
}
