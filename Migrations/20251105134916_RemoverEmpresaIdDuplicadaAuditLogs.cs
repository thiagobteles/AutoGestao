using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoGestao.Migrations
{
    /// <inheritdoc />
    public partial class RemoverEmpresaIdDuplicadaAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remover coluna duplicada empresa_id da tabela audit_logs (se existir)
            // A coluna correta é id_empresa (mapeada de BaseEntidade.IdEmpresa)
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM information_schema.columns
                        WHERE table_name = 'audit_logs'
                        AND column_name = 'empresa_id'
                    ) THEN
                        ALTER TABLE audit_logs DROP COLUMN empresa_id;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recriar coluna empresa_id caso precise fazer rollback
            migrationBuilder.AddColumn<long>(
                name: "empresa_id",
                table: "audit_logs",
                type: "bigint",
                nullable: true);
        }
    }
}
