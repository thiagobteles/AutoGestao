using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Enumerador.Veiculo;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Entidades.Veiculos
{
    [FormConfig(Title = "Veículo", Subtitle = "Gerencie todas as informações do veículo", Icon = "fas fa-car", EnableAjaxSubmit = true)]
    [FormTabs(EnableTabs = true, DefaultTab = "principal")]
    [FormTab("arquivos", "Arquivos", TabIcon = "fas fa-folder", Order = 1, Controller = "VeiculoDocumentos")]
    [FormTab("nfe", "NFE", TabIcon = "fas fa-file-invoice", Order = 2, Controller = "VeiculoNFE")]
    [FormTab("midias", "Mídias", TabIcon = "fas fa-images", Order = 3, Controller = "VeiculoFotos")]
    [FormTab("lancamentos", "Lançamentos", TabIcon = "fas fa-list", Order = 4, Controller = "VeiculoLancamentos")]
    [FormTab("despachante", "Despachante", TabIcon = "fas fa-user-tie", Order = 5, Controller = "VeiculoDespachante")]
    [FormTab("resumo", "Resumo", TabIcon = "fas fa-chart-line", Order = 6, Controller = "VeiculoResumo")]
    [FormTab("vistoria", "Vistoria", TabIcon = "fas fa-search", Order = 7, Controller = "VeiculoVistoria")]
    [FormTab("web", "Web", TabIcon = "fas fa-globe", Order = 8, Controller = "VeiculoWeb")]
    [FormTab("entrada", "Entrada", TabIcon = "fas fa-sign-in-alt", Order = 9, Controller = "VeiculoEntrada")]
    [FormTab("financeiro", "Financeiro", TabIcon = "fas fa-dollar-sign", Order = 11, Controller = "Despesas", RequiredRoles = new[] { "Admin", "Financeiro" })]
    public class Veiculo : BaseEntidade
    {
        [Required]
        [FormField(DisplayName = "Código", Icon = "fas fa-barcode", Type = EnumFormFieldType.Text, Required = true, Order = 1, Section = "Identificação")]
        public string Codigo { get; set; } = "";

        [Required]
        [FormField(DisplayName = "Placa", Icon = "fas fa-id-card", Type = EnumFormFieldType.Text, Required = true, Order = 2, Section = "Identificação", Placeholder = "XXX-0000 ou XXX0X00")]
        public string Placa { get; set; } = "";

        [FormField(DisplayName = "Marca", Icon = "fas fa-tag", Type = EnumFormFieldType.Select, Required = true, Order = 10, Section = "Especificações", GridColumns = 2)]
        public int VeiculoMarcaId { get; set; }

        [FormField(DisplayName = "Modelo", Icon = "fas fa-car-side", Type = EnumFormFieldType.Select, Required = true, Order = 11, Section = "Especificações", GridColumns = 2)]
        public int VeiculoMarcaModeloId { get; set; }

        [FormField(DisplayName = "Ano Fabricação", Icon = "fas fa-calendar", Type = EnumFormFieldType.Number, Required = true, Order = 12, Section = "Especificações", GridColumns = 2)]
        public int? AnoFabricacao { get; set; }

        [FormField(DisplayName = "Ano Modelo", Icon = "fas fa-calendar-check", Type = EnumFormFieldType.Number, Required = true, Order = 13, Section = "Especificações", GridColumns = 2)]
        public int? AnoModelo { get; set; }

        [FormField(DisplayName = "Cor", Icon = "fas fa-palette", Type = EnumFormFieldType.Select, Order = 14, Section = "Especificações")]
        public int? VeiculoCorId { get; set; }

        [FormField(DisplayName = "Situação", Icon = "fas fa-info-circle", Type = EnumFormFieldType.Select, Required = true, Order = 20, Section = "Status")]
        public EnumSituacaoVeiculo Situacao { get; set; }

        [FormField(DisplayName = "Status", Icon = "fas fa-info-circle", Type = EnumFormFieldType.Select, Required = true, Order = 21, Section = "Status")]
        public EnumStatusVeiculo StatusVeiculo { get; set; }

        [FormField(DisplayName = "Preço de Compra", Icon = "fas fa-money-bill", Type = EnumFormFieldType.Currency, Order = 30, Section = "Financeiro", GridColumns = 2)]
        public decimal? PrecoCompra { get; set; }

        [FormField(DisplayName = "Preço de Venda", Icon = "fas fa-dollar-sign", Type = EnumFormFieldType.Currency, Order = 31, Section = "Financeiro", GridColumns = 2)]
        public decimal? PrecoVenda { get; set; }

        [FormField(DisplayName = "Proprietário", Icon = "fas fa-user", Type = EnumFormFieldType.Select, Order = 40, Section = "Proprietário")]
        public int? ProprietarioId { get; set; }

        [FormField(DisplayName = "Observações", Icon = "fas fa-sticky-note", Type = EnumFormFieldType.TextArea, Order = 50, Section = "Observações")]
        public string? Observacoes { get; set; }

        [FormField(DisplayName = "Opcionais", Icon = "fas fa-sticky-note", Type = EnumFormFieldType.TextArea, Order = 51, Section = "Observações")]
        public string? Opcionais { get; set; }

        public EnumCombustivelVeiculo Combustivel { get; set; }
        public EnumCambioVeiculo Cambio { get; set; }
        public EnumTipoVeiculo TipoVeiculo { get; set; }
        public EnumEspecieVeiculo Especie { get; set; }
        public EnumPortasVeiculo Portas { get; set; }
        public EnumPericiaCautelarVeiculo PericiaCautelar { get; set; }
        public EnumOrigemVeiculo OrigemVeiculo { get; set; }
        public string? Motorizacao { get; set; }
        public long? KmSaida { get; set; }
        public string? Chassi { get; set; }
        public string? Renavam { get; set; }
        public int? Quilometragem { get; set; }
        public DateTime? DataSaida { get; set; }

        //// Foreign Keys
        public int? VeiculoFilialId { get; set; }
        public int? VeiculoLocalizacaoId { get; set; }

        // Propriedades de navegação (serão ignoradas automaticamente)
        public virtual VeiculoMarca? VeiculoMarca { get; set; }
        public virtual VeiculoMarcaModelo? VeiculoMarcaModelo { get; set; }
        public virtual Cliente? Proprietario { get; set; }
        public virtual VeiculoCor? VeiculoCor { get; set; }
        public virtual VeiculoFilial? VeiculoFilial { get; set; }
        public virtual VeiculoLocalizacao? VeiculoLocalizacao { get; set; }
        public virtual ICollection<Venda> Vendas { get; set; } = [];
        public virtual ICollection<VeiculoFoto> Fotos { get; set; } = [];
        public virtual ICollection<VeiculoDocumento> Documentos { get; set; } = [];
        public virtual ICollection<Despesa> Despesas { get; set; } = [];
    }
}