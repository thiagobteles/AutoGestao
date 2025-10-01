using AutoGestao.Attributes;
using AutoGestao.Enumerador;

namespace AutoGestao.Entidades
{
    public class Fornecedor : BaseEntidadeEmpresa
    {
        [GridField("Tipo", Order = 5, Width = "100px")]
        public EnumTipoPessoa TipoFornecedor { get; set; } = EnumTipoPessoa.Nenhum;

        [GridMain("Nome/Razão Social")]
        public string Nome { get; set; } = string.Empty;

        [GridDocument("CPF", DocumentType.CPF)]
        public string? CPF { get; set; }

        [GridDocument("CNPJ", DocumentType.CNPJ)]
        public string? CNPJ { get; set; }

        [GridField("RG", Order = 32, Width = "120px")]
        public string? RG { get; set; }

        [GridField("Data Nascimento", Order = 35, Width = "120px")]
        public DateTime? DataNascimento { get; set; }

        [GridContact("E-mail")]
        public string? Email { get; set; }

        [GridContact("Telefone")]
        public string? Telefone { get; set; }

        [GridField("Celular", IsSubtitle = true, SubtitleOrder = 3, Order = 65)]
        public string? Celular { get; set; }

        [GridField("Endereço", Order = 70, ShowInGrid = false)]
        public string? Endereco { get; set; }

        [GridField("Cidade", Order = 72)]
        public string? Cidade { get; set; }

        [GridField("Estado", Order = 75, Width = "65px")]
        public string? Estado { get; set; }

        [GridField("CEP", Order = 77, Width = "100px", Format = "#####-###")]
        public string? CEP { get; set; }

        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Observacoes { get; set; }
    }
}