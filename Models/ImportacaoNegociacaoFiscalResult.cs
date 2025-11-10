namespace FGT.Models
{
    public class ImportacaoNegociacaoFiscalResult
    {
        public bool Sucesso { get; set; }

        public string? Mensagem { get; set; }

        public int TotalLinhas { get; set; }

        public int LinhasImportadas { get; set; }

        public int LinhasComErro { get; set; }

        public List<string> Erros { get; set; } = [];

        public List<string> Avisos { get; set; } = [];
    }
}
