using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;

namespace AutoGestao.Entidades
{
    [Auditable(AuditCreate = true, AuditUpdate = true, AuditDelete = true)]
    public class BaseEntidadeDocumento : BaseEntidade
    {
        // ============================================================
        // TIPO PESSOA - Enum com ÍCONE na grid
        // ============================================================
        [GridField("Tipo", Order = 5, Width = "65px", EnumRender = EnumRenderType.Icon)]
        [FormField(Order = 1, Name = "Tipo de Cliente", Section = "Tipo Registro", Icon = "fas fa-user-tag", Type = EnumFieldType.Select, Required = true, GridColumns = 1)]
        public EnumTipoPessoa TipoPessoa { get; set; }

        [GridMain("Nome/Razão Social")]
        [FormField(Order = 10, Name = "Nome Completo", Section = "Dados Básicos", Icon = "fas fa-signature", Type = EnumFieldType.Text, Required = true, GridColumns = 2)]
        public string Nome { get; set; } = string.Empty;

        [ConditionalDisplay("TipoPessoa == 1")]
        [FormField(Order = 10, Name = "Data de nascimento", Section = "Dados Básicos", Icon = "fas fa-calendar", Type = EnumFieldType.Date)]
        public DateTime? DataNascimento { get; set; }

        // ============================================================
        // CPF - NÃO aparece na grid principal
        // Mas aparece como subtitle na busca de referência
        // ============================================================
        [GridDocument("CPF", DocumentType.CPF, ShowInGrid = false)]
        [ReferenceSubtitle(Order = 0, Prefix = "CPF: ", Format = "###.###.###-##")]
        [ConditionalDisplay("TipoPessoa == 1")]
        [FormField(Order = 10, Name = "CPF", Section = "Dados Básicos", Icon = "fas fa-fingerprint", Type = EnumFieldType.Cpf)]
        public string? Cpf { get; set; }

        [ConditionalDisplay("TipoPessoa == 1")]
        [FormField(Order = 10, Name = "RG", Section = "Dados Básicos", Icon = "fas fa-id-card", Type = EnumFieldType.Text)]
        public string? Rg { get; set; }

        // ============================================================
        // CNPJ - NÃO aparece na grid principal
        // Mas aparece como subtitle na busca de referência
        // ============================================================
        [GridDocument("CNPJ", DocumentType.CNPJ, ShowInGrid = false)]
        [ReferenceSubtitle(Order = 1, Prefix = "CNPJ: ", Format = "##.###.###/####-##")]
        [ConditionalDisplay("TipoPessoa == 2")]
        [FormField(Order = 10, Name = "CNPJ", Section = "Dados Básicos", Icon = "fas fa-building", Type = EnumFieldType.Cnpj)]
        public string? Cnpj { get; set; }

        // ============================================================
        // DOCUMENTO - APARECE NA GRID PRINCIPAL
        // Mas não existe a nível de banco
        // ============================================================
        [GridField("Documento")]
        public string Documento => $"{(Cpf != null ? Cpf.AplicarMascaraCpf() : Cnpj.AplicarMascaraCnpj())}";

        // ============================================================
        // EMAIL - NÃO aparece na grid principal
        // Mas aparece como subtitle na busca de referência
        // ============================================================
        [GridContact("E-mail", ShowInGrid = false, IsSubtitle = true, SubtitleOrder = 20)]
        [FormField(Order = 20, Name = "Email", Section = "Contato", Icon = "fas fa-envelope", Type = EnumFieldType.Email, GridColumns = 2)]
        public string? Email { get; set; }

        [GridContact("Telefone", ShowInGrid = false)]
        [FormField(Name = "Telefone", Section = "Contato", Icon = "fas fa-phone", Type = EnumFieldType.Telefone, Order = 21)]
        public string? Telefone { get; set; }

        // ============================================================
        // CELULAR - APARECE na grid e como subtitle
        // ============================================================
        [GridField("Celular", IsSubtitle = true, SubtitleOrder = 3, Order = 62)]
        [ConditionalRequired("IsEmpty(Telefone)", "Celular é obrigatório quando não há telefone")]
        [FormField(Name = "Celular", Section = "Contato", Icon = "fas fa-mobile", Type = EnumFieldType.Telefone, Order = 22)]
        public string? Celular { get; set; }
    }
}