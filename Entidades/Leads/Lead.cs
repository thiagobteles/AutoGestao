using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades.Leads
{
    [ReportConfig("Detalhes do lead", Icon = "fas fa-book", ShowLogo = true, ShowDate = true)]
    [FormConfig(Title = "Lead´s", Subtitle = "Gerencie os leads", Icon = "fas fa-book", EnableAjaxSubmit = true)]
    public class Lead : BaseEntidade
    {
        [ReferenceSearchable]
        [ReferenceText]
        [ReportField("Nome", Section = "Informações do contato", Order = 1, GridColumns = 1)]
        [GridMain("Nome", IsSubtitle = true, SubtitleOrder = 1, Order = 1)]
        [FormField(Order = 10, Name = "Nome Completo", Section = "Dados Básicos", Icon = "fas fa-signature", Type = EnumFieldType.Text, Required = true, GridColumns = 1)]
        public string Nome { get; set; } = string.Empty;

        [ReportField("Celular", Section = "Informações do contato", Order = 2)]
        [GridField("Celular", IsSubtitle = true, SubtitleOrder = 2, Order = 1)]
        [FormField(Order = 20, Name = "Celular", Section = "Contato", Icon = "fas fa-mobile", Type = EnumFieldType.Telefone, GridColumns = 2)]
        public string? Celular { get; set; }

        [ReportField("E-mail", Section = "Informações do contato", Order = 3)]
        [GridContact("E-mail", IsSubtitle = true, SubtitleOrder = 3, Order = 2)]
        [FormField(Order = 21, Name = "Email", Section = "Contato", Icon = "fas fa-envelope", Type = EnumFieldType.Email)]
        public string? Email { get; set; }

        [ReportField("Tipo Retorno", Section = "Retorno", Order = 1, GridColumns = 1)]
        [GridField("Tipo Retorno", Order = 3, EnumRender = EnumRenderType.IconDescription)]
        [FormField(Order = 30, Name = "Tipo retorno", Section = "Retorno", Icon = "fas fa-refresh", Type = EnumFieldType.Select, GridColumns = 2)]
        public EnumTipoRetornoContato TipoRetornoContato { get; set; } = EnumTipoRetornoContato.Whatsapp;

        [ReportField("Status", Section = "Retorno", Order = 1)]
        [GridField("Status", Order = 4, EnumRender = EnumRenderType.IconDescription)]
        [FormField(Order = 31, Name = "Status", Section = "Retorno", Icon = "fas fa-info-circle", Type = EnumFieldType.Select)]
        public EnumStatusLead Status { get; set; } = EnumStatusLead.Pendente;

        [ReportField("Contexto", Section = "Dados do contato", Order = 1, GridColumns = 1)]
        [FormField(Order = 40, Name = "Contexto", Section = "Contexto", Icon = "fas fa-commenting", Type = EnumFieldType.TextArea, GridColumns = 1)]
        public string? Contexto { get; set; }

        [GridStatus(ShowInGrid = false)]
        public new bool Ativo { get; set; } = true;
    }
}