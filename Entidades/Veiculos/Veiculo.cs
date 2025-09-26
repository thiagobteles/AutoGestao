using AutoGestao.Attributes;
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
        [FormField(DisplayName = "Código", Icon = "fas fa-barcode", Type = EnumFieldType.Number, Required = false, ReadOnly = true, Order = 1, Section = "Identificação", GridColumns = 2)]
        public string Codigo { get; set; } = "";

        [Required]
        [FormField(DisplayName = "Placa", Icon = "fas fa-id-card", Type = EnumFieldType.Text, Required = true, Order = 2, Section = "Identificação", Placeholder = "XXX-0000 ou XXX0X00", GridColumns = 2)]
        public string Placa { get; set; } = "";

        [FormField(Order = 14, DisplayName = "Chassi", Type = EnumFieldType.Text, Section = "Identificação", GridColumns = 2)]
        public string? Chassi { get; set; }

        [FormField(Order = 14, DisplayName = "Renavam", Type = EnumFieldType.Text, Section = "Identificação", GridColumns = 2)]
        public string? Renavam { get; set; }

        [FormField(DisplayName = "Marca", Icon = "fas fa-car", Order = 10, Section = "Especificações", Required = true, Type = EnumFieldType.Reference, Reference = typeof(VeiculoMarca), Placeholder = "Buscar Marca...")]
        public int VeiculoMarcaId { get; set; }

        [FormField(DisplayName = "Modelo", Icon = "fas fa-car-side", Required = true, Order = 11, Section = "Especificações", Type = EnumFieldType.Reference, Reference = typeof(VeiculoMarcaModelo), ConditionalField = nameof(VeiculoMarcaId), Placeholder = "Buscar modelo...")]
        public int VeiculoMarcaModeloId { get; set; }

        [FormField(DisplayName = "Ano Fabricação", Icon = "fas fa-calendar", Type = EnumFieldType.Number, Required = true, Order = 12, Section = "Especificações", GridColumns = 2)]
        public int? AnoFabricacao { get; set; }

        [FormField(DisplayName = "Ano Modelo", Icon = "fas fa-calendar-check", Type = EnumFieldType.Number, Required = true, Order = 13, Section = "Especificações", GridColumns = 2)]
        public int? AnoModelo { get; set; }

        [FormField(DisplayName = "Cor", Icon = "fas fa-palette", Order = 14, Section = "Especificações", Type = EnumFieldType.Reference, Reference = typeof(VeiculoCor), Placeholder = "Buscar cor...")]
        public int? VeiculoCorId { get; set; }

        [FormField(Order = 14, DisplayName = "Motorização", Type = EnumFieldType.Text, Section = "Especificações", GridColumns = 2)]
        public string? Motorizacao { get; set; }

        [FormField(Order = 14, DisplayName = "Quilometragem", Type = EnumFieldType.Text, Section = "Especificações", GridColumns = 2)]
        public int? Quilometragem { get; set; }

        [FormField(Order = 15, DisplayName = "Combustível", Type = EnumFieldType.Select, Section = "Especificações", GridColumns = 2)]
        public EnumCombustivelVeiculo Combustivel { get; set; }

        [FormField(Order = 16, DisplayName = "Câmbio", Type = EnumFieldType.Select, Section = "Especificações", GridColumns = 2)]
        public EnumCambioVeiculo Cambio { get; set; }

        [FormField(Order = 17, DisplayName = "Tipo", Type = EnumFieldType.Select, Section = "Especificações", GridColumns = 2)]
        public EnumTipoVeiculo TipoVeiculo { get; set; }

        [FormField(Order = 18, DisplayName = "Espécie", Type = EnumFieldType.Select, Section = "Especificações", GridColumns = 2)]
        public EnumEspecieVeiculo Especie { get; set; }

        [FormField(Order = 19, DisplayName = "Portas", Type = EnumFieldType.Select, Section = "Especificações", GridColumns = 2)]
        public EnumPortasVeiculo Portas { get; set; }

        [FormField(Order = 20, DisplayName = "Perícia Cautelar", Type = EnumFieldType.Select, Section = "Especificações", GridColumns = 2)]
        public EnumPericiaCautelarVeiculo PericiaCautelar { get; set; }

        [FormField(Order = 21, DisplayName = "Origem", Type = EnumFieldType.Select, Section = "Especificações", GridColumns = 2)]
        public EnumOrigemVeiculo OrigemVeiculo { get; set; }

        [FormField(DisplayName = "Preço de Compra", Icon = "fas fa-money-bill", Type = EnumFieldType.Currency, Order = 40, Section = "Financeiro", GridColumns = 2)]
        public decimal? PrecoCompra { get; set; }

        [FormField(DisplayName = "Preço de Venda", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Currency, Order = 41, Section = "Financeiro", GridColumns = 2)]
        public decimal? PrecoVenda { get; set; }

        [FormField(DisplayName = "Proprietário", Icon = "fas fa-user", Type = EnumFieldType.Reference, Reference = typeof(Cliente), Order = 42, Section = "Financeiro", Placeholder = "Buscar cliente...")]
        public int? ProprietarioId { get; set; }

        [FormField(DisplayName = "Km saida", Type = EnumFieldType.Number, Order = 43, Section = "Financeiro", GridColumns = 2)]
        public long? KmSaida { get; set; }

        [FormField(DisplayName = "Data saída", Type = EnumFieldType.Date, Order = 44, Section = "Financeiro", GridColumns = 2)]
        public DateTime? DataSaida { get; set; }

        [FormField(DisplayName = "Situação", Icon = "fas fa-info-circle", Type = EnumFieldType.Select, Required = true, Order = 30, Section = "Informativo", GridColumns = 2)]
        public EnumSituacaoVeiculo Situacao { get; set; }

        [FormField(DisplayName = "Status", Icon = "fas fa-info-circle", Type = EnumFieldType.Select, Required = true, Order = 31, Section = "Informativo")]
        public EnumStatusVeiculo StatusVeiculo { get; set; }

        [FormField(DisplayName = "Filial", Order = 32, Section = "Informativo", Type = EnumFieldType.Reference, Reference = typeof(VeiculoFilial), Placeholder = "Buscar filial...")]
        public int? VeiculoFilialId { get; set; }

        [FormField(DisplayName = "Localização", Order = 33, Section = "Informativo", Type = EnumFieldType.Reference, Reference = typeof(VeiculoLocalizacao), Placeholder = "Buscar localização...")]
        public int? VeiculoLocalizacaoId { get; set; }

        [FormField(DisplayName = "Observações", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Order = 34, Section = "Informativo")]
        public string? Observacoes { get; set; }

        [FormField(DisplayName = "Opcionais", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Order = 35, Section = "Informativo")]
        public string? Opcionais { get; set; }

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