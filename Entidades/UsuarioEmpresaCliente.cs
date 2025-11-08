using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades
{
    /// <summary>
    /// Tabela de relacionamento N:N entre Usuario e EmpresaCliente
    /// Permite que um usuário tenha acesso a múltiplas empresas clientes
    /// </summary>
    [Auditable(EntityDisplayName = "Vínculo Usuário-Empresa")]
    [FormConfig(Title = "Vínculo Usuário-Empresa", Subtitle = "Gerencie o acesso de usuários às empresas clientes", Icon = "fas fa-link")]
    [Table("usuario_empresa_cliente")]
    public class UsuarioEmpresaCliente : IEntity
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("id_usuario")]
        [FormField(Name = "Usuário", Order = 1, Section = "Dados do Vínculo", Icon = "fas fa-user", Type = EnumFieldType.Reference, Required = true, Reference = typeof(Usuario))]
        public long IdUsuario { get; set; }

        [Required]
        [Column("id_empresa_cliente")]
        [FormField(Name = "Empresa Cliente", Order = 2, Section = "Dados do Vínculo", Icon = "fas fa-building", Type = EnumFieldType.Reference, Required = true, Reference = typeof(Fiscal.EmpresaCliente))]
        public long IdEmpresaCliente { get; set; }

        [Column("data_vinculo")]
        [GridField("Data do Vínculo", Order = 20, Width = "150px")]
        [FormField(Name = "Data do Vínculo", Order = 3, Section = "Dados do Vínculo", Icon = "fas fa-calendar", Type = EnumFieldType.DateTime, ReadOnly = true)]
        public DateTime DataVinculo { get; set; } = DateTime.UtcNow;

        [Column("ativo")]
        [GridField("Status", Order = 30, Width = "100px")]
        [FormField(Name = "Ativo", Order = 4, Section = "Dados do Vínculo", Icon = "fas fa-toggle-on", Type = EnumFieldType.Checkbox)]
        public bool Ativo { get; set; } = true;

        // Navigation properties
        [ForeignKey("IdUsuario")]
        public virtual Usuario? Usuario { get; set; }

        [GridComposite("Empresa Cliente", Order = 10, NavigationPaths = new[] { "EmpresaCliente.RazaoSocial", "EmpresaCliente.CNPJ" },
            Template = @"<div class=""vehicle-info""><div class=""fw-semibold"">{0}</div><div class=""text-muted small"">CNPJ: {1}</div></div>")]
        [ForeignKey("IdEmpresaCliente")]
        public virtual Fiscal.EmpresaCliente? EmpresaCliente { get; set; }

        public long IdEmpresa { get; set; }
    }
}
