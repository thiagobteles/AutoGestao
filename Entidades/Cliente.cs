using AutoGestao.Attributes;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

[FormConfig(Title = "Cliente", Subtitle = "Gerencie as informações dos clientes", Icon = "fas fa-user", EnableAjaxSubmit = true)]
public class Cliente : BaseEntidadeEmpresa
{
    [GridField("Tipo", Order = 5, Width = "100px")]
    [FormField(Order = 1, Name = "Tipo de Cliente", Section = "Tipo Registro", Icon = "fas fa-user-tag", Type = EnumFieldType.Select, Required = true, GridColumns = 1)]
    public EnumTipoPessoa TipoCliente { get; set; }

    [GridMain("Nome/Razão Social")]
    [FormField(Order = 10, Name = "Nome Completo", Section = "Dados Básicos", Icon = "fas fa-signature", Type = EnumFieldType.Text, Required = true, GridColumns = 2)]
    public string Nome { get; set; } = string.Empty;

    [GridField("Data Nascimento", Order = 35, Width = "120px")]
    [ConditionalDisplay("TipoCliente == 1")]
    [FormField(Order = 10, Name = "Data de nascimento", Section = "Dados Básicos", Icon = "fas fa-building", Type = EnumFieldType.Date)]
    public DateTime? DataNascimento { get; set; }

    [GridDocument("CPF", DocumentType.CPF)]
    [ConditionalDisplay("TipoCliente == 1")]
    [FormField(Order = 10, Name = "CPF", Section = "Dados Básicos", Icon = "fas fa-fingerprint", Type = EnumFieldType.Cpf)]
    public string? Cpf { get; set; }

    [GridField("RG", Order = 32, Width = "120px")]
    [ConditionalDisplay("TipoCliente == 1")]
    [FormField(Order = 10, Name = "RG", Section = "Dados Básicos", Icon = "fas fa-building", Type = EnumFieldType.Text)]
    public string? Rg { get; set; }

    [GridDocument("CNPJ", DocumentType.CNPJ)]
    [ConditionalDisplay("TipoCliente == 2")]
    [FormField(Order = 10, Name = "CNPJ", Section = "Dados Básicos", Icon = "fas fa-building", Type = EnumFieldType.Cnpj)]
    public string? Cnpj { get; set; }

    [GridContact("E-mail")]
    [FormField(Order = 20, Name = "Email", Section = "Contato", Icon = "fas fa-envelope", Type = EnumFieldType.Email, GridColumns = 2)]
    public string? Email { get; set; }

    [GridContact("Telefone")]
    [FormField(Name = "Telefone", Section = "Contato", Icon = "fas fa-phone", Type = EnumFieldType.Phone, Order = 21)]
    public string? Telefone { get; set; }

    [GridField("Celular", IsSubtitle = true, SubtitleOrder = 3, Order = 62)]
    [ConditionalRequired("IsEmpty(Telefone)", "Celular é obrigatório quando não há telefone")]
    [FormField(Name = "Celular", Section = "Contato", Icon = "fas fa-mobile", Type = EnumFieldType.Phone, Order = 22)]
    public string? Celular { get; set; }

    [GridField("CEP", Order = 70, Width = "100px", Format = "#####-###")]
    [FormField(Order = 30, Name = "CEP", Section = "Endereço", Icon = "fas fa-mail-bulk", Type = EnumFieldType.Cep, GridColumns = 3)]
    public string? CEP { get; set; }

    [GridField("Estado", Order = 75, Width = "65px")]
    [FormField(Order = 30, Name = "Estado", Section = "Endereço", Icon = "fas fa-flag", Type = EnumFieldType.Select)]
    public EnumEstado Estado { get; set; }

    [GridField("Cidade", Order = 72)]
    [FormField(Order = 30, Name = "Cidade", Section = "Endereço", Icon = "fas fa-city", Type = EnumFieldType.Text)]
    public string? Cidade { get; set; }

    [FormField(Order = 30, Name = "Endereço", Section = "Endereço", Icon = "fas fa-road", Type = EnumFieldType.Text)]
    public string? Endereco { get; set; }

    [FormField(Order = 30, Name = "Número", Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.Text)]
    public string? Numero { get; set; }

    [FormField(Order = 30, Name = "Bairro", Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.Text)]
    public string? Bairro { get; set; }

    [FormField(Order = 30, Name = "Complemento", Section = "Endereço", Icon = "fas fa-hashtag", Type = EnumFieldType.TextArea, Placeholder = "Complemento do endereço...")]
    public string? Complemento { get; set; }

    [FormField(Order = 40, Name = "Observações", Section = "Status", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Informações adicionais sobre o cliente...")]
    public string? Observacoes { get; set; }

    // Navigation properties
    public virtual ICollection<Venda> Vendas { get; set; } = [];
    public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = [];
    public virtual ICollection<Veiculo> Veiculos { get; set; } = [];
}