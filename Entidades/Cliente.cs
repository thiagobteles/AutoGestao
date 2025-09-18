using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;

namespace AutoGestao.Entidades
{
    public class Cliente : BaseEntidade
    {
        public EnumTipoPessoa TipoCliente { get; set; } = EnumTipoPessoa.Nenhum;
        public string Nome { get; set; } = string.Empty;
        public string? CPF { get; set; }
        public string? CNPJ { get; set; }
        public string? RG { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Celular { get; set; }
        public string? Endereco { get; set; }
        public string? Cidade { get; set; }
        public EnumEstado Estado { get; set; } = EnumEstado.Nenhum;
        public string? CEP { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; } 
        public bool Ativo { get; set; } = true;
        public string? Observacoes { get; set; }

        // Navigation properties
        public virtual ICollection<Veiculo> Veiculos { get; set; } = [];
        public virtual ICollection<Venda> Vendas { get; set; } = [];
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = [];
    }
}