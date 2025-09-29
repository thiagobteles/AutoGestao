using AutoGestao.Enumerador;

namespace AutoGestao.Entidades.Veiculos
{
    public class VeiculoDocumento : BaseEntidadeEmpresa
    {
        public EnumTipoDocumento TipoDocumento { get; set; } = EnumTipoDocumento.Nenhum;
        public string NomeArquivo { get; set; } = string.Empty;
        public string CaminhoArquivo { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public DateTime DataUpload { get; set; }

        // Foreign Keys
        public int VeiculoId { get; set; }

        // Navigation properties
        public virtual Veiculo Veiculo { get; set; } = null!;
    }
}