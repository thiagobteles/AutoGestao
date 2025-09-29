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
    public class Veiculo : BaseEntidadeEmpresa
    {
        [FormField(Order = 1, Name = "Código", Section = "Identificação", Icon = "fas fa-barcode", Type = EnumFieldType.Number, ReadOnly = true, GridColumns = 2)]
        public string Codigo { get; set; }

        [Required]
        [FormField(Order = 1, Name = "Placa", Section = "Identificação", Icon = "fas fa-id-card", Type = EnumFieldType.Text, Required = true, Placeholder = "XXX-0000 ou XXX0X00")]
        public string Placa { get; set; }

        [FormField(Order = 1, Name = "Chassi", Section = "Identificação", Type = EnumFieldType.Text)]
        public string? Chassi { get; set; }

        [FormField(Order = 1, Name = "Renavam", Section = "Identificação", Type = EnumFieldType.Text)]
        public string? Renavam { get; set; }

        [FormField(Order = 10, Name = "Marca", Section = "Especificações", Icon = "fas fa-car", Required = true, Type = EnumFieldType.Reference, Reference = typeof(VeiculoMarca), Placeholder = "Buscar Marca...", GridColumns = 2)]
        public int VeiculoMarcaId { get; set; }

        [FormField(Order = 10, Name = "Modelo", Section = "Especificações", Icon = "fas fa-car-side", Required = true, Type = EnumFieldType.Reference, Reference = typeof(VeiculoMarcaModelo), ConditionalField = nameof(VeiculoMarcaId), Placeholder = "Buscar modelo...")]
        public int VeiculoMarcaModeloId { get; set; }

        [FormField(Order = 10, Name = "Ano Fabricação", Section = "Especificações", Icon = "fas fa-calendar", Type = EnumFieldType.Number, Required = true)]
        public int? AnoFabricacao { get; set; }

        [FormField(Order = 10, Name = "Ano Modelo", Section = "Especificações", Icon = "fas fa-calendar-check", Type = EnumFieldType.Number, Required = true)]
        public int? AnoModelo { get; set; }

        [FormField(Order = 10, Name = "Cor", Section = "Especificações", Icon = "fas fa-palette", Type = EnumFieldType.Reference, Reference = typeof(VeiculoCor), Placeholder = "Buscar cor...")]
        public int? VeiculoCorId { get; set; }

        [FormField(Order = 10, Name = "Motorização", Section = "Especificações", Type = EnumFieldType.Text)]
        public string? Motorizacao { get; set; }

        [FormField(Order = 10, Name = "Quilometragem", Section = "Especificações", Type = EnumFieldType.Text)]
        public int? Quilometragem { get; set; }

        [FormField(Order = 10, Name = "Combustível", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumCombustivelVeiculo Combustivel { get; set; }

        [FormField(Order = 10, Name = "Câmbio", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumCambioVeiculo Cambio { get; set; }

        [FormField(Order = 10, Name = "Tipo", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumTipoVeiculo TipoVeiculo { get; set; }

        [FormField(Order = 10, Name = "Espécie", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumEspecieVeiculo Especie { get; set; }

        [FormField(Order = 10, Name = "Portas", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumPortasVeiculo NumeroPortas { get; set; }

        [FormField(Order = 10, Name = "Capacidade porta malas", Section = "Especificações", Type = EnumFieldType.Text)]
        public string CapacidadePortaMalas { get; set; }

        [FormField(Order = 10, Name = "Perícia Cautelar", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumPericiaCautelarVeiculo PericiaCautelar { get; set; }

        [FormField(Order = 10, Name = "Origem", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumOrigemVeiculo OrigemVeiculo { get; set; }

        [FormField(Order = 20, Name = "Proprietário", Section = "Financeiro", Icon = "fas fa-user", Type = EnumFieldType.Reference, Reference = typeof(Cliente), GridColumns = 2)]
        public int? ProprietarioId { get; set; }

        [FormField(Order = 20, Name = "Data entrada", Section = "Financeiro", Type = EnumFieldType.Date)]
        public DateTime? DataEntrada { get; set; }

        [FormField(Order = 20, Name = "Preço de Compra", Section = "Financeiro", Icon = "fas fa-money-bill", Type = EnumFieldType.Currency)]
        public decimal? PrecoCompra { get; set; }

        [FormField(Order = 20, Name = "Preço de Venda", Section = "Financeiro", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Currency)]
        public decimal? PrecoVenda { get; set; }

        [FormField(Order = 20, Name = "Km saida", Section = "Financeiro", Type = EnumFieldType.Number)]
        public long? KmSaida { get; set; }

        [FormField(Order = 20, Name = "Data saída", Section = "Financeiro", Type = EnumFieldType.Date)]
        public DateTime? DataSaida { get; set; }

        [FormField(Order = 30, Name = "Status", Section = "Status", Icon = "fas fa-info-circle", Type = EnumFieldType.Select, Required = true, GridColumns = 2)]
        public EnumStatusVeiculo Status { get; set; }

        [FormField(Order = 30, Name = "Situação", Section = "Status", Icon = "fas fa-info-circle", Type = EnumFieldType.Select, Required = true)]
        public EnumSituacaoVeiculo Situacao { get; set; }

        [FormField(Order = 40, Name = "Filial", Section = "Informativo", Type = EnumFieldType.Reference, Reference = typeof(VeiculoFilial), GridColumns = 2)]
        public int? VeiculoFilialId { get; set; }

        [FormField(Order = 40, Name = "Localização", Section = "Informativo", Type = EnumFieldType.Reference, Reference = typeof(VeiculoLocalizacao))]
        public int? VeiculoLocalizacaoId { get; set; }

        [FormField(Order = 40, Name = "Observações", Section = "Informativo", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea)]
        public string? Observacoes { get; set; }

        [FormField(Order = 40, Name = "Opcionais", Section = "Informativo", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea)]
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