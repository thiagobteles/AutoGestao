using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Fiscal;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades.Fiscal
{
    [FormConfig(Title = "Parâmetro Fiscal", Subtitle = "Configure os parâmetros fiscais para emissão de notas", Icon = "fas fa-cogs")]
    public class ParametroFiscal : BaseEntidade
    {
        [FormField(Name = "Empresa Cliente", Order = 10, Section = "Dados Principais", Icon = "fas fa-building", Type = EnumFieldType.Reference, Required = true, Reference = typeof(EmpresaCliente))]
        [Required]
        public long EmpresaClienteId { get; set; }

        [GridComposite("Empresa", Order = 10, NavigationPaths = new[] { "EmpresaCliente.RazaoSocial", "EmpresaCliente.CNPJ" },
            Template = @"<div class=""vehicle-info""><div class=""fw-semibold"">{0}</div><div class=""text-muted small"">{1}</div></div>")]
        public string EmpresaClienteNome => $"{EmpresaCliente?.RazaoSocial ?? "N/A"} - {EmpresaCliente?.CNPJ ?? "N/A"}";

        [GridField("Ambiente NFe", Order = 15, Width = "120px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Name = "Ambiente de Emissão NFe", Order = 15, Section = "Configurações NFe", Icon = "fas fa-server", Type = EnumFieldType.Select, Required = true)]
        [Required]
        public EnumAmbienteNFe AmbienteNFe { get; set; } = EnumAmbienteNFe.Homologacao;

        [GridField("Série NFe", Order = 20, Width = "80px")]
        [FormField(Name = "Série Padrão NFe", Order = 20, Section = "Configurações NFe", Icon = "fas fa-list-ol", Type = EnumFieldType.Number, GridColumns = 4)]
        public int SerieNFe { get; set; } = 1;

        [GridField("Próximo Número NFe", Order = 25, Width = "120px")]
        [FormField(Name = "Próximo Número NFe", Order = 25, Section = "Configurações NFe", Icon = "fas fa-hashtag", Type = EnumFieldType.Number, GridColumns = 4)]
        public int ProximoNumeroNFe { get; set; } = 1;

        [GridField("Série NFSe", Order = 30, Width = "80px")]
        [FormField(Name = "Série Padrão NFSe", Order = 30, Section = "Configurações NFSe", Icon = "fas fa-list-ol", Type = EnumFieldType.Number, GridColumns = 4)]
        public int SerieNFSe { get; set; } = 1;

        [GridField("Próximo Número NFSe", Order = 35, Width = "120px")]
        [FormField(Name = "Próximo Número NFSe", Order = 35, Section = "Configurações NFSe", Icon = "fas fa-hashtag", Type = EnumFieldType.Number, GridColumns = 4)]
        public int ProximoNumeroNFSe { get; set; } = 1;

        [FormField(Name = "CFOP Padrão Venda", Order = 40, Section = "CFOPs Padrão", Icon = "fas fa-file-contract", Type = EnumFieldType.Text, Placeholder = "Ex: 5.102", GridColumns = 3)]
        [MaxLength(6)]
        public string? CfopPadraoVenda { get; set; }

        [FormField(Name = "CFOP Padrão Compra", Order = 45, Section = "CFOPs Padrão", Icon = "fas fa-file-contract", Type = EnumFieldType.Text, Placeholder = "Ex: 1.102", GridColumns = 3)]
        [MaxLength(6)]
        public string? CfopPadraoCompra { get; set; }

        [FormField(Name = "CFOP Padrão Serviço", Order = 50, Section = "CFOPs Padrão", Icon = "fas fa-file-contract", Type = EnumFieldType.Text, Placeholder = "Ex: 5.933", GridColumns = 3)]
        [MaxLength(6)]
        public string? CfopPadraoServico { get; set; }

        [FormField(Name = "CST ICMS Padrão", Order = 55, Section = "Tributação Padrão", Icon = "fas fa-percentage", Type = EnumFieldType.Text, Placeholder = "Ex: 00", GridColumns = 4)]
        [MaxLength(3)]
        public string? CstIcmsPadrao { get; set; }

        [FormField(Name = "CST PIS Padrão", Order = 60, Section = "Tributação Padrão", Icon = "fas fa-percentage", Type = EnumFieldType.Text, Placeholder = "Ex: 01", GridColumns = 4)]
        [MaxLength(2)]
        public string? CstPisPadrao { get; set; }

        [FormField(Name = "CST COFINS Padrão", Order = 65, Section = "Tributação Padrão", Icon = "fas fa-percentage", Type = EnumFieldType.Text, Placeholder = "Ex: 01", GridColumns = 4)]
        [MaxLength(2)]
        public string? CstCofinsPadrao { get; set; }

        [FormField(Name = "Mensagem Padrão NFe", Order = 70, Section = "Mensagens Padrão", Icon = "fas fa-comment", Type = EnumFieldType.TextArea, Placeholder = "Mensagem que aparecerá em todas as NFes...", GridColumns = 1)]
        [MaxLength(500)]
        public string? MensagemPadraoNFe { get; set; }

        [FormField(Name = "Mensagem Padrão NFSe", Order = 75, Section = "Mensagens Padrão", Icon = "fas fa-comment", Type = EnumFieldType.TextArea, Placeholder = "Mensagem que aparecerá em todas as NFSes...", GridColumns = 1)]
        [MaxLength(500)]
        public string? MensagemPadraoNFSe { get; set; }

        [FormField(Name = "Enviar Email Automático", Order = 80, Section = "Configurações de Email", Icon = "fas fa-envelope", Type = EnumFieldType.Checkbox)]
        public bool EnviarEmailAutomatico { get; set; } = true;

        [FormField(Name = "Email Padrão Cópia", Order = 85, Section = "Configurações de Email", Icon = "fas fa-at", Type = EnumFieldType.Email, Placeholder = "contabilidade@empresa.com.br")]
        [MaxLength(100)]
        public string? EmailPadraoCopia { get; set; }

        // Navigation
        [ForeignKey("EmpresaClienteId")]
        public virtual EmpresaCliente? EmpresaCliente { get; set; }
    }
}
