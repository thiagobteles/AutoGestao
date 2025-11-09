using AutoGestao.Atributes;
using AutoGestao.Entidades.Base;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Fiscal;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Empresa Cliente", Subtitle = "Gerencie as empresas dos seus clientes contábeis", Icon = "fas fa-building")]
    public class EmpresaCliente : BaseEntidade
    {
        [GridField("Razão Social", Order = 10)]
        [FormField(Name = "Razão Social", Order = 10, Section = "Dados Principais", Icon = "fas fa-building", Type = EnumFieldType.Text, Required = true, Placeholder = "Razão social da empresa...")]
        [Required(ErrorMessage = "Razão social é obrigatória")]
        [ReferenceText]
        [ReferenceSearchable]
        [MaxLength(200)]
        public string RazaoSocial { get; set; } = string.Empty;

        [GridField("Nome Fantasia", Order = 15)]
        [FormField(Name = "Nome Fantasia", Order = 15, Section = "Dados Principais", Icon = "fas fa-store", Type = EnumFieldType.Text, Placeholder = "Nome fantasia...")]
        [ReferenceSearchable]
        [MaxLength(200)]
        public string? NomeFantasia { get; set; }

        [GridField("CNPJ", Order = 20, Width = "150px")]
        [FormField(Name = "CNPJ", Order = 20, Section = "Dados Principais", Icon = "fas fa-id-card", Type = EnumFieldType.Cnpj, Required = true)]
        [Required(ErrorMessage = "CNPJ é obrigatório")]
        [ReferenceSubtitle(Order = 0, Prefix = "CNPJ: ", Format = "##.###.###/####-##")] // Format será usado automaticamente como replace quando for Searchable!
        [ReferenceSearchable]
        [MaxLength(18)]
        public string CNPJ { get; set; } = string.Empty;

        [GridField("Inscrição Estadual", Order = 25, ShowInGrid = false)]
        [FormField(Name = "Inscrição Estadual", Order = 25, Section = "Dados Principais", Icon = "fas fa-file-contract", Type = EnumFieldType.Text, GridColumns = 3)]
        [MaxLength(20)]
        public string? InscricaoEstadual { get; set; }

        [FormField(Name = "Inscrição Municipal", Order = 26, Section = "Dados Principais", Icon = "fas fa-file-signature", Type = EnumFieldType.Text, GridColumns = 3)]
        [MaxLength(20)]
        public string? InscricaoMunicipal { get; set; }

        [GridField("Regime", Order = 30, EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Regime Tributário", Order = 30, Section = "Dados Fiscais", Icon = "fas fa-balance-scale", Type = EnumFieldType.Select, Required = true)]
        [Required(ErrorMessage = "Regime tributário é obrigatório")]
        public EnumRegimeTributario RegimeTributario { get; set; }

        [FormField(Name = "CNAE Principal", Order = 35, Section = "Dados Fiscais", Icon = "fas fa-industry", Type = EnumFieldType.Reference, Reference = typeof(CNAE), Placeholder = "Selecione o CNAE principal...")]
        public long? CNAEPrincipalId { get; set; }

        [FormField(Name = "Contador Responsável", Order = 37, Section = "Dados Fiscais", Icon = "fas fa-user-tie", Type = EnumFieldType.Reference, Reference = typeof(ContadorResponsavel), Placeholder = "Selecione o contador responsável...")]
        public long? ContadorResponsavelId { get; set; }

        [FormField(Name = "Email", Order = 40, Section = "Contato", Icon = "fas fa-envelope", Type = EnumFieldType.Email)]
        [MaxLength(100)]
        public string? Email { get; set; }

        [FormField(Name = "Telefone", Order = 45, Section = "Contato", Icon = "fas fa-phone", Type = EnumFieldType.Telefone)]
        [MaxLength(20)]
        public string? Telefone { get; set; }

        [FormField(Name = "CEP", Order = 50, Section = "Endereço", Icon = "fas fa-mail-bulk", Type = EnumFieldType.Cep, GridColumns = 3)]
        [MaxLength(9)]
        public string? CEP { get; set; }

        [FormField(Name = "Estado", Order = 55, Section = "Endereço", Icon = "fas fa-flag", Type = EnumFieldType.Select, Required = true)]
        public EnumEstado Estado { get; set; }

        [FormField(Name = "Cidade", Order = 60, Section = "Endereço", Icon = "fas fa-city", Type = EnumFieldType.Text)]
        [MaxLength(100)]
        public string? Cidade { get; set; }

        [FormField(Name = "Endereço", Order = 65, Section = "Endereço", Icon = "fas fa-road", Type = EnumFieldType.Text)]
        [MaxLength(200)]
        public string? Endereco { get; set; }

        [FormField(Name = "Número", Order = 70, Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.Text, GridColumns = 3)]
        [MaxLength(10)]
        public string? Numero { get; set; }

        [FormField(Name = "Complemento", Order = 75, Section = "Endereço", Icon = "fas fa-info-circle", Type = EnumFieldType.Text)]
        [MaxLength(100)]
        public string? Complemento { get; set; }

        [FormField(Name = "Observações", Order = 110, Section = "Informações Adicionais", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Observações gerais sobre a empresa...", GridColumns = 1)]
        [MaxLength(1000)]
        public string? Observacoes { get; set; }

        // Navigation properties
        [ForeignKey("CNAEPrincipalId")]
        public virtual CNAE? CNAEPrincipal { get; set; }

        [ForeignKey("ContadorResponsavelId")]
        public virtual ContadorResponsavel? ContadorResponsavel { get; set; }
        public virtual ICollection<NotaFiscal> NotasFiscais { get; set; } = [];
        public virtual ICollection<CertificadoDigital> Certificados { get; set; } = [];
        public virtual ICollection<ParametroFiscal> ParametrosFiscais { get; set; } = [];
        public virtual ICollection<DadoBancario> DadosBancarios { get; set; } = [];
        public virtual ICollection<ObrigacaoFiscal> ObrigacoesFiscais { get; set; } = [];
        public virtual ICollection<LancamentoContabil> Lancamentos { get; set; } = [];
    }
}
