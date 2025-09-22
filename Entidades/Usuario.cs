using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Usuário", Subtitle = "Gerencie os usuários do sistema", Icon = "fas fa-users", EnableAjaxSubmit = true)]
    public class Usuario : BaseEntidade
    {
        [Required]
        [FormField(DisplayName = "Nome Completo", Icon = "fas fa-user", Type = FormFieldType.Text, Required = true, Order = 1, Section = "Dados Básicos")]
        public string Nome { get; set; } = "";

        [Required]
        [EmailAddress]
        [FormField(DisplayName = "Email/Login", Icon = "fas fa-envelope", Type = FormFieldType.Email, Required = true, Order = 2, Section = "Dados Básicos")]
        public string Email { get; set; } = "";

        [Required]
        [FormField(DisplayName = "Senha", Icon = "fas fa-lock", Type = FormFieldType.Password, Required = true, Order = 3, Section = "Dados Básicos")]
        public string SenhaHash { get; set; } = "";

        [FormField(DisplayName = "CPF", Icon = "fas fa-fingerprint", Type = FormFieldType.Cpf, Order = 4, Section = "Dados Básicos")]
        public string? CPF { get; set; }

        [FormField(DisplayName = "Telefone", Icon = "fas fa-phone", Type = FormFieldType.Phone, Order = 5, Section = "Contato")]
        public string? Telefone { get; set; }

        [FormField(DisplayName = "Perfil", Icon = "fas fa-user-tag", Type = FormFieldType.Select, Required = true, Order = 10, Section = "Permissões")]
        public EnumPerfilUsuario Perfil { get; set; } = EnumPerfilUsuario.Vendedor;

        [FormField(DisplayName = "Ativo", Icon = "fas fa-toggle-on", Type = FormFieldType.Checkbox, Order = 11, Section = "Permissões")]
        public bool Ativo { get; set; } = true;

        [FormField(DisplayName = "Último Login", Icon = "fas fa-clock", Type = FormFieldType.DateTime, ReadOnly = true, Order = 20, Section = "Informações")]
        public DateTime? UltimoLogin { get; set; }

        [FormField(DisplayName = "Observações", Icon = "fas fa-sticky-note", Type = FormFieldType.TextArea, Order = 30, Section = "Observações")]
        public string? Observacoes { get; set; }

        // Propriedades auxiliares não mapeadas
        [FormField(DisplayName = "Confirmar Senha", Icon = "fas fa-lock", Type = FormFieldType.Password, Order = 4, Section = "Dados Básicos")]
        public string? ConfirmarSenha { get; set; }

        // Navigation properties
        public virtual ICollection<Tarefa> TarefasResponsavel { get; set; } = [];
    }
}