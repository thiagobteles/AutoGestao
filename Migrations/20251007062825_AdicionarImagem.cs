using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarImagem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "foto",
                table: "veiculo_fotos",
                type: "character varying(10000)",
                maxLength: 10000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "foto",
                table: "veiculo_fotos");
        }
    }
}
