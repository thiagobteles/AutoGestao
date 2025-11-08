using AutoGestao.Atributes;
using AutoGestao.Entidades.Fiscal;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades
{
    [Auditable(EntityDisplayName = "Usuário")]
    [FormConfig(Title = "Usuário", Subtitle = "Gerencie os usuários do sistema", Icon = "fas fa-users")]
    public class Usuario : BaseEntidade
    {
        [ReferenceText]
        [GridMain("Nome")]
        [FormField(Name = "Nome Completo", Order = 1, Section = "Dados Básicos", Icon = "fas fa-user", Type = EnumFieldType.Text, Required = true, GridColumns = 2)]
        public string Nome { get; set; } = "";

        [ReferenceSubtitle(Order = 1, Prefix = "Email: ")]
        [GridContact("E-mail/Login")]
        [FormField(Name = "Email/Login", Order = 2, Section = "Dados Básicos", Icon = "fas fa-envelope", Type = EnumFieldType.Email, Required = true)]
        public string Email { get; set; } = "";

        [GridDocument("CPF", DocumentType.CPF)]
        [FormField(Name = "CPF", Order = 3, Section = "Dados Básicos", Icon = "fas fa-fingerprint", Type = EnumFieldType.Cpf)]
        public string? Cpf { get; set; }

        [ReferenceSubtitle(Order = 0, Prefix = "Tel: ")]
        [GridField("Telefone", IsSubtitle = true, SubtitleOrder = 2, Order = 60)]
        [FormField(Name = "Telefone", Order = 4, Section = "Dados Básicos", Icon = "fas fa-phone", Type = EnumFieldType.Telefone)]
        public string? Telefone { get; set; }

        [FormField(Name = "Empresa Padrão", Order = 9, Section = "Vínculo Empresarial", Icon = "fas fa-building", Type = EnumFieldType.Reference, Reference = typeof(EmpresaCliente), HelpText = "Empresa padrão do usuário. Para múltiplas empresas, use a tab 'Empresas Vinculadas'", GridColumns = 1)]
        public long? IdEmpresaCliente { get; set; }

        [GridField("Perfil", Order = 10, Width = "120px")]
        [FormField(Name = "Perfil", Order = 10, Section = "Informações", Icon = "fas fa-user-tag", Type = EnumFieldType.Select, Required = true, GridColumns = 2)]
        public EnumPerfilUsuario Perfil { get; set; }

        [GridField("Último Login", Order = 11, Width = "140px")]
        [FormField(Name = "Último Login", Order = 11, Section = "Informações", Icon = "fas fa-clock", Type = EnumFieldType.DateTime, ReadOnly = true)]
        public DateTime? UltimoLogin { get; set; }

        [AuditSensitive(MaskPattern = "***")]
        [FormField(Name = "Senha", Order = 20, Section = "Login", Icon = "fas fa-lock", Type = EnumFieldType.Password, Required = true)]
        public string SenhaHash { get; set; } = "";

        [FormField(Name = "Confirmar Senha", Order = 21, Section = "Login", Icon = "fas fa-lock", Type = EnumFieldType.Password)]
        public string? ConfirmarSenha { get; set; }

        [FormField(Name = "Observações", Order = 30, Section = "Observações", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, GridColumns = 1)]
        public string? Observacoes { get; set; }

        // Navigation properties
        [ForeignKey("IdEmpresaCliente")]
        public virtual EmpresaCliente? EmpresaCliente { get; set; }

        /// <summary>
        /// Relacionamento N:N com EmpresaCliente
        /// Permite que um usuário tenha acesso a múltiplas empresas
        /// </summary>
        public virtual ICollection<UsuarioEmpresaCliente> EmpresasVinculadas { get; set; } = [];

        public virtual ICollection<AuditLog> AuditLogs { get; set; } = [];
        public virtual ICollection<BaseEntidade> EntidadesCriadas { get; set; } = [];
        public virtual ICollection<BaseEntidade> EntidadesAlteradas { get; set; } = [];
    }
}