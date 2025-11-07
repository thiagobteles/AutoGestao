using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Fiscal;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades.Fiscal
{
    [FormConfig(Title = "Dado Bancário", Subtitle = "Gerencie as contas bancárias das empresas clientes", Icon = "fas fa-university")]
    public class DadoBancario : BaseEntidade
    {
        [FormField(Name = "Empresa Cliente", Order = 10, Section = "Dados Principais", Icon = "fas fa-building", Type = EnumFieldType.Reference, Required = true, Reference = typeof(EmpresaCliente))]
        [Required]
        public long EmpresaClienteId { get; set; }

        [GridComposite("Empresa", Order = 10, NavigationPaths = new[] { "EmpresaCliente.RazaoSocial", "EmpresaCliente.CNPJ" },
            Template = @"<div class=""vehicle-info""><div class=""fw-semibold"">{0}</div><div class=""text-muted small"">{1}</div></div>")]
        public string EmpresaClienteNome => $"{EmpresaCliente?.RazaoSocial ?? "N/A"} - {EmpresaCliente?.CNPJ ?? "N/A"}";

        [GridField("Banco", Order = 15, Width = "200px")]
        [FormField(Name = "Nome do Banco", Order = 15, Section = "Dados Bancários", Icon = "fas fa-university", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(100)]
        public string NomeBanco { get; set; } = string.Empty;

        [GridField("Código", Order = 20, Width = "80px")]
        [FormField(Name = "Código do Banco", Order = 20, Section = "Dados Bancários", Icon = "fas fa-hashtag", Type = EnumFieldType.Text, Required = true, GridColumns = 4, Placeholder = "Ex: 001, 237...")]
        [Required]
        [MaxLength(3)]
        public string CodigoBanco { get; set; } = string.Empty;

        [GridField("Agência", Order = 25, Width = "100px")]
        [FormField(Name = "Agência", Order = 25, Section = "Dados Bancários", Icon = "fas fa-building-columns", Type = EnumFieldType.Text, Required = true, GridColumns = 4)]
        [Required]
        [MaxLength(10)]
        public string Agencia { get; set; } = string.Empty;

        [FormField(Name = "Dígito da Agência", Order = 30, Section = "Dados Bancários", Icon = "fas fa-key", Type = EnumFieldType.Text, GridColumns = 4)]
        [MaxLength(2)]
        public string? DigitoAgencia { get; set; }

        [GridField("Conta", Order = 35, Width = "120px")]
        [FormField(Name = "Número da Conta", Order = 35, Section = "Dados Bancários", Icon = "fas fa-wallet", Type = EnumFieldType.Text, Required = true, GridColumns = 3)]
        [Required]
        [MaxLength(20)]
        public string NumeroConta { get; set; } = string.Empty;

        [GridField("Dígito", Order = 40, Width = "60px")]
        [FormField(Name = "Dígito da Conta", Order = 40, Section = "Dados Bancários", Icon = "fas fa-key", Type = EnumFieldType.Text, Required = true, GridColumns = 3)]
        [Required]
        [MaxLength(2)]
        public string DigitoConta { get; set; } = string.Empty;

        [GridField("Tipo", Order = 45, Width = "120px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Tipo de Conta", Order = 45, Section = "Dados Bancários", Icon = "fas fa-money-check", Type = EnumFieldType.Select, Required = true, GridColumns = 3)]
        [Required]
        public EnumTipoConta TipoConta { get; set; }

        [FormField(Name = "PIX (Chave)", Order = 50, Section = "Pagamentos Digitais", Icon = "fas fa-qrcode", Type = EnumFieldType.Text, Placeholder = "CPF/CNPJ, email, telefone ou chave aleatória...")]
        [MaxLength(100)]
        public string? ChavePix { get; set; }

        [GridField("Principal", Order = 55, Width = "100px")]
        [FormField(Name = "Conta Principal", Order = 55, Section = "Configurações", Icon = "fas fa-star", Type = EnumFieldType.Checkbox)]
        public bool ContaPrincipal { get; set; } = false;

        [FormField(Name = "Observações", Order = 60, Section = "Informações Adicionais", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Observações sobre esta conta...", GridColumns = 1)]
        [MaxLength(500)]
        public string? Observacoes { get; set; }

        // Navigation
        [ForeignKey("EmpresaClienteId")]
        public virtual EmpresaCliente? EmpresaCliente { get; set; }
    }
}
