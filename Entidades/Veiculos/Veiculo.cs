using AutoGestao.Enumerador.Veiculo;

namespace AutoGestao.Entidades.Veiculos
{
    public class Veiculo : BaseEntidade
    {
        public string Codigo { get; set; } = string.Empty;
        public EnumCombustivelVeiculo Combustivel { get; set; } = EnumCombustivelVeiculo.Nenhum;
        public EnumCambioVeiculo Cambio { get; set; } = EnumCambioVeiculo.Nenhum;
        public EnumSituacaoVeiculo Situacao { get; set; } = EnumSituacaoVeiculo.Estoque;
        public EnumStatusVeiculo StatusVeiculo { get; set; } = EnumStatusVeiculo.Nenhum;
        public EnumTipoVeiculo TipoVeiculo { get; set; } = EnumTipoVeiculo.Nenhum;
        public EnumEspecieVeiculo Especie { get; set; } = EnumEspecieVeiculo.Automovel;
        public EnumPortasVeiculo Portas { get; set; } = EnumPortasVeiculo.Nenhuma;
        public EnumPericiaCautelarVeiculo PericiaCautelar { get; set; } = EnumPericiaCautelarVeiculo.Nenhuma;
        public string Motorizacao { get; set; } = "2.0";
        public int AnoFabricacao { get; set; }
        public int AnoModelo { get; set; }
        public string Placa { get; set; } = string.Empty;
        public long KmSaida { get; set; }
        public string? Chassi { get; set; }
        public string? Renavam { get; set; }
        public int? Quilometragem { get; set; }
        public decimal? PrecoCompra { get; set; }
        public decimal? PrecoVenda { get; set; }
        public string? Observacoes { get; set; }
        public string? Opcionais { get; set; }
        public DateTime? DataSaida { get; set; }
        public DateTime? DataAlteracao { get; set; }

        // Foreign Keys
        public int? ProprietarioId { get; set; }
        public int? VeiculoCorId { get; set; }
        public int? VeiculoFilialId { get; set; }
        public int? VeiculoLocalizacaoId { get; set; }
        public int? VeiculoMarcaId { get; set; }
        public int? VeiculoMarcaModeloId { get; set; }


        // Navigation properties
        public virtual Cliente? Proprietario { get; set; }
        public virtual VeiculoCor? Cor { get; set; }
        public virtual VeiculoFilial? Filial { get; set; }
        public virtual VeiculoLocalizacao? Localizacao { get; set; }
        public virtual VeiculoMarca? Marca { get; set; }
        public virtual VeiculoMarcaModelo? Modelo { get; set; }
        public virtual ICollection<Venda> Vendas { get; set; } = [];
        public virtual ICollection<VeiculoFoto> Fotos { get; set; } = [];
        public virtual ICollection<VeiculoDocumento> Documentos { get; set; } = [];
        public virtual ICollection<Despesa> Despesas { get; set; } = [];
    }
}