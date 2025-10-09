using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class Abas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nome_arquivo",
                table: "veiculo_documentos");

            migrationBuilder.RenameColumn(
                name: "caminho_arquivo",
                table: "veiculo_documentos",
                newName: "documento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "documento",
                table: "veiculo_documentos",
                newName: "caminho_arquivo");

            migrationBuilder.AddColumn<string>(
                name: "nome_arquivo",
                table: "veiculo_documentos",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
