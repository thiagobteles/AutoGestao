using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarUsuarioEmpresaCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuario_empresa_cliente",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<long>(type: "bigint", nullable: false),
                    id_empresa_cliente = table.Column<long>(type: "bigint", nullable: false),
                    data_vinculo = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuario_empresa_cliente", x => x.id);
                    table.ForeignKey(
                        name: "FK_usuario_empresa_cliente_empresas_clientes_id_empresa_cliente",
                        column: x => x.id_empresa_cliente,
                        principalTable: "empresas_clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuario_empresa_cliente_usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "usuarios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_usuario_empresa_cliente_id_empresa_cliente",
                table: "usuario_empresa_cliente",
                column: "id_empresa_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_empresa_cliente_id_usuario",
                table: "usuario_empresa_cliente",
                column: "id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuario_empresa_cliente");
        }
    }
}
