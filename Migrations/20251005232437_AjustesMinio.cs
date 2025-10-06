using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class AjustesMinio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "documento_rg",
                table: "clientes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "foto_cliente",
                table: "clientes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "documento_rg",
                table: "clientes");

            migrationBuilder.DropColumn(
                name: "foto_cliente",
                table: "clientes");
        }
    }
}
