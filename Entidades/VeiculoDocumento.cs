namespace AutoGestao.Entidades
{
    public class VeiculoDocumento
    {
        public int Id { get; set; }
        public string TipoDocumento { get; set; } = string.Empty; // CRV, CRLV, NotaFiscal, etc.
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