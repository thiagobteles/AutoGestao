using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Veiculo;

namespace AutoGestao.Entidades
{
    public class Veiculo
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public EnumMarcaVeiculo Marca { get; set; } = EnumMarcaVeiculo.Nenhum;
        public EnumCor Cor { get; set; } = EnumCor.Nenhum;
        public EnumCombustivelVeiculo Combustivel { get; set; } = EnumCombustivelVeiculo.Nenhum;
        public EnumCambioVeiculo Cambio { get; set; } = EnumCambioVeiculo.Nenhum;
        public EnumSituacaoVeiculo Situacao { get; set; } = EnumSituacaoVeiculo.Estoque;
        public EnumStatusVeiculo StatusVeiculo { get; set; } = EnumStatusVeiculo.Nenhum;
        public EnumTipoVeiculo TipoVeiculo { get; set; } = EnumTipoVeiculo.Nenhum;
        public EnumEspecieVeiculo Especie { get; set; } = EnumEspecieVeiculo.Automovel;
        public EnumEmpresa Filial { get; set; } = EnumEmpresa.Filial;
        public EnumLocalizacaoVeiculo Localizacao { get; set; } = EnumLocalizacaoVeiculo.Patio;
        public EnumPortasVeiculo Portas { get; set; } = EnumPortasVeiculo.Nenhuma;
        public EnumPericiaCautelarVeiculo PericiaCautelar { get; set; } = EnumPericiaCautelarVeiculo.Nenhuma;
        public string Modelo { get; set; } = string.Empty;
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
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public DateTime? DataSaida { get; set; }

        // Foreign Keys
        public int? ProprietarioId { get; set; }

        // Navigation properties
        public virtual Cliente? Proprietario { get; set; }
        public virtual ICollection<Venda> Vendas { get; set; } = [];
        public virtual ICollection<VeiculoFoto> Fotos { get; set; } = [];
        public virtual ICollection<VeiculoDocumento> Documentos { get; set; } = [];
        public virtual ICollection<Despesa> Despesas { get; set; } = [];
    }
}