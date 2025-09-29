namespace AutoGestao.Entidades.Veiculos
{
    public class VeiculoFoto : BaseEntidadeEmpresa
    {
        public string NomeArquivo { get; set; } = string.Empty;
        public string CaminhoArquivo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public DateTime DataUpload { get; set; }
        public bool Principal { get; set; }

        // Foreign Keys
        public int VeiculoId { get; set; }

        // Navigation properties
        public virtual Veiculo Veiculo { get; set; } = null!;
    }
}