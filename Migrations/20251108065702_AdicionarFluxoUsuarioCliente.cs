using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGT.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarFluxoUsuarioCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "id_empresa",
                table: "usuario_empresa_cliente",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_empresa",
                table: "usuario_empresa_cliente");
        }
    }
}
