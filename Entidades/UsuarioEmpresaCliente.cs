using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades
{
    /// <summary>
    /// Tabela de relacionamento N:N entre Usuario e EmpresaCliente
    /// Permite que um usuário tenha acesso a múltiplas empresas clientes
    /// </summary>
    [Table("usuario_empresa_cliente")]
    public class UsuarioEmpresaCliente
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("id_usuario")]
        public long IdUsuario { get; set; }

        [Required]
        [Column("id_empresa_cliente")]
        public long IdEmpresaCliente { get; set; }

        [Column("data_vinculo")]
        public DateTime DataVinculo { get; set; } = DateTime.UtcNow;

        [Column("ativo")]
        public bool Ativo { get; set; } = true;

        // Navigation properties
        [ForeignKey("IdUsuario")]
        public virtual Usuario? Usuario { get; set; }

        [ForeignKey("IdEmpresaCliente")]
        public virtual Fiscal.EmpresaCliente? EmpresaCliente { get; set; }
    }
}
