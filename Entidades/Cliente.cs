namespace AutoGestao.Entidades
{
    public class Cliente
    {
        public int Id { get; set; }
        public string TipoCliente { get; set; } = string.Empty; // PessoaFisica, PessoaJuridica
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
        public string? Estado { get; set; }
        public string? CEP { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; } 
        public string Status { get; set; } = "Ativo";
        public string? Observacoes { get; set; }
        public DateTime DataCadastro { get; set; }

        // Navigation properties
        public virtual ICollection<Veiculo> Veiculos { get; set; } = [];
        public virtual ICollection<Venda> Vendas { get; set; } = [];
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = [];
    }
}