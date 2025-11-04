using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Enumerador.Veiculo;

namespace AutoGestao.Entidades.Veiculos
{
   
    [FormConfig(Title = "Veículo", Subtitle = "Gerencie todas as informações do veículo", Icon = "fas fa-car", EnableAjaxSubmit = true)] // Anotação para geração da GRID
    [FormTabs(EnableTabs = true, DefaultTab = "principal")] // Anotações para geração de TABS
    [FormTab("arquivos", "Arquivos", TabIcon = "fas fa-folder", Order = 1, Controller = "VeiculoDocumento", LazyLoad = true)]
    [FormTab("midias", "Mídias", TabIcon = "fas fa-images", Order = 3, Controller = "VeiculoFoto", LazyLoad = true)]
    [FormTab("financeiro", "Financeiro", TabIcon = "fas fa-dollar-sign", Order = 6, Controller = "Despesa", RequiredRoles = new[] { "Admin", "Financeiro" })]
    public class Veiculo : BaseEntidade
    {
        [GridId()] // Campo aparece na GRID
        [FormField(Order = 1, Name = "Código", Section = "Identificação", Icon = "fas fa-barcode", Type = EnumFieldType.Text, ReadOnly = true, Required = false, GridColumns = 2)] // Campo aparece no CADASTRO/EDIÇÃO
        public string Codigo { get; set; } = string.Empty;

        // ============================================================
        // MARCA + MODELO - Campo COMPOSTO para grid
        // Combina VeiculoMarca.Descricao + VeiculoMarcaModelo.Descricao
        // ============================================================
        [GridComposite("Veículo", Order = 20, Width = "200px", NavigationPaths = new[] { "VeiculoMarca.Descricao", "VeiculoMarcaModelo.Descricao" },
            Template = @"<div class=""vehicle-info""><div class=""fw-semibold"">{0}</div><div class=""text-muted small"">{1}</div></div>")]
        public string MarcaModelo => $"{VeiculoMarca?.Descricao ?? "N/A"} - {VeiculoMarcaModelo?.Descricao ?? "N/A"}";

        // ============================================================
        // ANO - Campo composto para grid (Fabricação/Modelo)
        // ============================================================
        [GridField("Ano", Order = 30, Width = "100px")]
        public string AnoComposto => $"{AnoFabricacao}/{AnoModelo}";

        [ReferenceText]
        [ReferenceSearchable]
        [GridField("Placa", Order = 40, Width = "120px")]
        [FormField(Order = 1, Name = "Placa", Section = "Identificação", Icon = "fas fa-id-card", Type = EnumFieldType.Text, Required = true, Placeholder = "XXX-0000 ou XXX0X00")]
        public string Placa { get; set; } = string.Empty;

        [GridField("Situação", Order = 50, Width = "100px", EnumRender = EnumRenderType.IconDescription)]
        [FormField(Order = 30, Name = "Situação", Section = "Status", Icon = "fas fa-info-circle", Type = EnumFieldType.Select, Required = true)]
        public EnumSituacaoVeiculo Situacao { get; set; } = EnumSituacaoVeiculo.Estoque;

        [FormField(Order = 1, Name = "Chassi", Section = "Identificação", Type = EnumFieldType.Text)]
        public string? Chassi { get; set; }

        [FormField(Order = 1, Name = "Renavam", Section = "Identificação", Type = EnumFieldType.Text)]
        public string? Renavam { get; set; }

        [FormField(Order = 10, Name = "Marca", Section = "Especificações", Icon = "fas fa-car", Required = true, Type = EnumFieldType.Reference, Reference = typeof(VeiculoMarca), Placeholder = "Buscar Marca...", GridColumns = 2)]
        public long IdVeiculoMarca { get; set; }

        // ============================================================
        // (Anotação: ConditionalRule) => Habilitado condicionalmente apenas se Marca estiver selecionada
        // (Anotação: ReferenceFilter) => Quando habilitado é utilizado o campo IdVeiculoMarca dessa classe para filtrar o IdVeiculoMarca dentro da referencia VeiculoMarcaModelo 
        // ============================================================
        [ConditionalRule(EnumConditionalRuleType.Enabled, "IdVeiculoMarca != 0")]
        [ReferenceFilter("IdVeiculoMarca", "IdVeiculoMarca", Operator = EnumFilterOperator.Equals)]
        [FormField(Order = 10, Name = "Modelo", Section = "Especificações", Icon = "fas fa-car-side", Required = true, Type = EnumFieldType.Reference, Reference = typeof(VeiculoMarcaModelo), Placeholder = "Buscar modelo...")]
        public long IdVeiculoMarcaModelo { get; set; }

        [FormField(Order = 10, Name = "Ano Fabricação", Section = "Especificações", Icon = "fas fa-calendar", Type = EnumFieldType.Number, Required = true)]
        public int? AnoFabricacao { get; set; }

        [FormField(Order = 10, Name = "Ano Modelo", Section = "Especificações", Icon = "fas fa-calendar-check", Type = EnumFieldType.Number, Required = true)]
        public int? AnoModelo { get; set; }

        [FormField(Order = 10, Name = "Cor", Section = "Especificações", Icon = "fas fa-palette", Type = EnumFieldType.Reference, Reference = typeof(VeiculoCor), Placeholder = "Buscar cor...")]
        public long? IdVeiculoCor { get; set; }

        [FormField(Order = 10, Name = "Motorização", Section = "Especificações", Type = EnumFieldType.Text)]
        public string? Motorizacao { get; set; }

        [FormField(Order = 10, Name = "Capacidade porta malas", Section = "Especificações", Type = EnumFieldType.Number)]
        public int? CapacidadePortaMalas { get; set; }

        [FormField(Order = 10, Name = "Combustível", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumCombustivelVeiculo Combustivel { get; set; } = EnumCombustivelVeiculo.Flex;

        [FormField(Order = 10, Name = "Câmbio", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumCambioVeiculo Cambio { get; set; } = EnumCambioVeiculo.Manual;

        [FormField(Order = 10, Name = "Tipo", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumTipoVeiculo TipoVeiculo { get; set; } = EnumTipoVeiculo.Proprio;

        [FormField(Order = 10, Name = "Espécie", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumEspecieVeiculo Especie { get; set; } = EnumEspecieVeiculo.Automovel;

        [FormField(Order = 10, Name = "Portas", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumPortasVeiculo NumeroPortas { get; set; } = EnumPortasVeiculo.Duas;

        [FormField(Order = 10, Name = "Perícia Cautelar", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumPericiaCautelarVeiculo PericiaCautelar { get; set; } = EnumPericiaCautelarVeiculo.Aprovado;

        [FormField(Order = 10, Name = "Origem", Section = "Especificações", Type = EnumFieldType.Select)]
        public EnumOrigemVeiculo OrigemVeiculo { get; set; } = EnumOrigemVeiculo.Nacional;

        [FormField(Order = 20, Name = "Preço de Compra", Section = "Financeiro", Icon = "fas fa-money-bill", Type = EnumFieldType.Currency)]
        public decimal? PrecoCompra { get; set; }

        [FormField(Order = 20, Name = "Proprietário", Section = "Financeiro", Icon = "fas fa-user", Type = EnumFieldType.Reference, Reference = typeof(Cliente), GridColumns = 2)]
        public long IdCliente { get; set; }

        [FormField(Order = 20, Name = "Data entrada", Section = "Financeiro", Type = EnumFieldType.Date)]
        public DateTime? DataEntrada { get; set; } = DateTime.Now;

        [FormField(Order = 20, Name = "Km entrada", Section = "Financeiro", Type = EnumFieldType.Decimal)]
        public decimal? KmEntrada { get; set; }

        [FormField(Order = 20, Name = "Data saída", Section = "Financeiro", Type = EnumFieldType.Date)]
        public DateTime? DataSaida { get; set; }

        [FormField(Order = 20, Name = "Km saida", Section = "Financeiro", Type = EnumFieldType.Decimal)]
        public decimal? KmSaida { get; set; }

        [GridField("Preço", Order = 60, Width = "140px", Format = "C")]
        [FormField(Order = 20, Name = "Preço de Venda", Section = "Financeiro", Icon = "fas fa-dollar-sign", Type = EnumFieldType.Currency)]
        public decimal? PrecoVenda { get; set; }

        [FormField(Order = 30, Name = "Status", Section = "Status", Icon = "fas fa-info-circle", Type = EnumFieldType.Select, Required = true, GridColumns = 2)]
        public EnumStatusVeiculo Status { get; set; } = EnumStatusVeiculo.Usado;

        [FormField(Order = 40, Name = "Filial", Section = "Informativo", Type = EnumFieldType.Reference, Reference = typeof(VeiculoFilial), GridColumns = 2)]
        public long? IdVeiculoFilial { get; set; }

        [FormField(Order = 40, Name = "Localização", Section = "Informativo", Type = EnumFieldType.Reference, Reference = typeof(VeiculoLocalizacao))]
        public long? IdVeiculoLocalizacao { get; set; }

        [FormField(Order = 40, Name = "Observações", Section = "Informativo", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea)]
        public string? Observacoes { get; set; }

        [FormField(Order = 40, Name = "Opcionais", Section = "Informativo", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea)]
        public string? Opcionais { get; set; }

        // Navigation properties (ignoradas automaticamente)
        public virtual VeiculoMarca? VeiculoMarca { get; set; }
        public virtual VeiculoMarcaModelo? VeiculoMarcaModelo { get; set; }
        public virtual Cliente? Cliente { get; set; }
        public virtual VeiculoCor? VeiculoCor { get; set; }
        public virtual VeiculoFilial? VeiculoFilial { get; set; }
        public virtual VeiculoLocalizacao? VeiculoLocalizacao { get; set; }
        public virtual ICollection<VeiculoFoto> Fotos { get; set; } = [];
        public virtual ICollection<VeiculoDocumento> Documentos { get; set; } = [];
    }
}