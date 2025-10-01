using AutoGestao.Atributes;
using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Entidades
{
    [Auditable(EntityDisplayName = "Usuário")]
    [FormConfig(Title = "Usuário", Subtitle = "Gerencie os usuários do sistema", Icon = "fas fa-users", EnableAjaxSubmit = true)]
    public class Usuario : BaseEntidadeEmpresa
    {
        [GridMain("Nome")]
        [FormField(Name = "Nome Completo", Icon = "fas fa-user", Type = EnumFieldType.Text, Required = true, Order = 1, Section = "Dados Básicos")]
        public string Nome { get; set; } = "";

        [GridContact("E-mail/Login")]
        [FormField(Name = "Email/Login", Icon = "fas fa-envelope", Type = EnumFieldType.Email, Required = true, Order = 2, Section = "Dados Básicos")]
        public string Email { get; set; } = "";

        [AuditSensitive(MaskPattern = "***")]
        [FormField(Name = "Senha", Icon = "fas fa-lock", Type = EnumFieldType.Password, Required = true, Order = 3, Section = "Dados Básicos")]
        public string SenhaHash { get; set; } = "";

        [FormField(Name = "Confirmar Senha", Icon = "fas fa-lock", Type = EnumFieldType.Password, Order = 4, Section = "Dados Básicos")]
        public string? ConfirmarSenha { get; set; }

        [GridDocument("CPF", DocumentType.CPF)]
        [FormField(Name = "CPF", Icon = "fas fa-fingerprint", Type = EnumFieldType.Cpf, Order = 4, Section = "Dados Básicos")]
        public string? CPF { get; set; }

        [GridField("Telefone", IsSubtitle = true, SubtitleOrder = 2, Order = 60)]
        [FormField(Name = "Telefone", Icon = "fas fa-phone", Type = EnumFieldType.Phone, Order = 5, Section = "Contato")]
        public string? Telefone { get; set; }

        [GridField("Perfil", Order = 85, Width = "120px")]
        [FormField(Name = "Perfil", Icon = "fas fa-user-tag", Type = EnumFieldType.Select, Required = true, Order = 10, Section = "Permissões")]
        public EnumPerfilUsuario Perfil { get; set; }

        [GridField("Último Login", Order = 87, Width = "140px")]
        [FormField(Name = "Último Login", Icon = "fas fa-clock", Type = EnumFieldType.DateTime, ReadOnly = true, Order = 20, Section = "Informações")]
        public DateTime? UltimoLogin { get; set; }

        [FormField(Name = "Observações", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Order = 30, Section = "Observações")]
        public string? Observacoes { get; set; }

        // Navigation properties
        public virtual ICollection<Tarefa> TarefasResponsavel { get; set; } = [];
        public virtual ICollection<AuditLog> AuditLogs { get; set; } = [];
        public virtual ICollection<BaseEntidade> EntidadesCriadas { get; set; } = [];
        public virtual ICollection<BaseEntidade> EntidadesAlteradas { get; set; } = [];
    }
}