using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class AlteracoesVeiculo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "quilometragem",
                table: "veiculos");

            migrationBuilder.AddColumn<decimal>(
                name: "km_entrada",
                table: "veiculos",
                type: "numeric(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "km_entrada",
                table: "veiculos");

            migrationBuilder.AddColumn<int>(
                name: "quilometragem",
                table: "veiculos",
                type: "integer",
                nullable: true);
        }
    }
}
