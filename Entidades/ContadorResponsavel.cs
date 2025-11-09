using AutoGestao.Atributes;
using AutoGestao.Entidades.Base;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Contador Responsável", Subtitle = "Cadastro dos contadores responsáveis pelas empresas", Icon = "fas fa-user-tie")]
    public class ContadorResponsavel : BaseEntidade
    {
        [GridField("Nome", Order = 10)]
        [FormField(Name = "Nome Completo", Order = 10, Section = "Dados Pessoais", Icon = "fas fa-user", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(200)]
        public string Nome { get; set; } = string.Empty;

        [GridField("CPF", Order = 15, Width = "150px")]
        [FormField(Name = "CPF", Order = 15, Section = "Dados Pessoais", Icon = "fas fa-id-card", Type = EnumFieldType.Cpf, Required = true, GridColumns = 3)]
        [Required]
        [MaxLength(14)]
        public string CPF { get; set; } = string.Empty;

        [GridField("CRC", Order = 20, Width = "150px")]
        [FormField(Name = "CRC (Registro no Conselho)", Order = 20, Section = "Dados Profissionais", Icon = "fas fa-certificate", Type = EnumFieldType.Text, Required = true, GridColumns = 3)]
        [Required]
        [MaxLength(20)]
        public string CRC { get; set; } = string.Empty;

        [FormField(Name = "Estado do CRC", Order = 25, Section = "Dados Profissionais", Icon = "fas fa-flag", Type = EnumFieldType.Select, Required = true, GridColumns = 3)]
        [Required]
        public EnumEstado EstadoCRC { get; set; }

        [GridField("Email", Order = 30)]
        [FormField(Name = "Email Profissional", Order = 30, Section = "Contato", Icon = "fas fa-envelope", Type = EnumFieldType.Email, Required = true)]
        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [GridField("Telefone", Order = 35, Width = "150px")]
        [FormField(Name = "Telefone", Order = 35, Section = "Contato", Icon = "fas fa-phone", Type = EnumFieldType.Telefone, GridColumns = 3)]
        [MaxLength(20)]
        public string? Telefone { get; set; }

        [FormField(Name = "Celular", Order = 40, Section = "Contato", Icon = "fas fa-mobile-alt", Type = EnumFieldType.Telefone, GridColumns = 3)]
        [MaxLength(20)]
        public string? Celular { get; set; }

        [FormField(Name = "Escritório/Empresa", Order = 45, Section = "Dados Profissionais", Icon = "fas fa-briefcase", Type = EnumFieldType.Text)]
        [MaxLength(200)]
        public string? Escritorio { get; set; }

        [FormField(Name = "CNPJ do Escritório", Order = 50, Section = "Dados Profissionais", Icon = "fas fa-id-card-alt", Type = EnumFieldType.Cnpj, GridColumns = 3)]
        [MaxLength(18)]
        public string? CNPJEscritorio { get; set; }

        [FormField(Name = "Observações", Order = 60, Section = "Informações Adicionais", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Observações sobre o contador...", GridColumns = 1)]
        [MaxLength(500)]
        public string? Observacoes { get; set; }

        // Navigation properties
        public virtual ICollection<EmpresaCliente> EmpresasClientes { get; set; } = [];
    }
}
