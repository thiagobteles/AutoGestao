using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Lead´s", Subtitle = "Gerencie os leads", Icon = "fas fa-book")]
    public class Lead : BaseEntidade
    {
        [ReferenceSearchable]
        [ReferenceText]
        [GridMain("Nome", IsSubtitle = true, SubtitleOrder = 1, Order = 1)]
        [FormField(Order = 10, Name = "Nome Completo", Section = "Dados Básicos", Icon = "fas fa-signature", Type = EnumFieldType.Text, Required = true, GridColumns = 1)]
        public string Nome { get; set; } = string.Empty;

        [GridField("Celular", IsSubtitle = true, SubtitleOrder = 2, Order = 1)]
        [FormField(Order = 20, Name = "Celular", Section = "Contato", Icon = "fas fa-mobile", Type = EnumFieldType.Telefone, GridColumns = 2)]
        public string? Celular { get; set; }

        [GridContact("E-mail", IsSubtitle = true, SubtitleOrder = 3, Order = 2)]
        [FormField(Order = 21, Name = "Email", Section = "Contato", Icon = "fas fa-envelope", Type = EnumFieldType.Email)]
        public string? Email { get; set; }

        [GridField("Tipo Retorno", Order = 3, EnumRender = EnumRenderType.IconDescription)]
        [FormField(Order = 30, Name = "Tipo retorno", Section = "Retorno", Icon = "fas fa-refresh", Type = EnumFieldType.Select, GridColumns = 2)]
        public EnumTipoRetornoContato TipoRetornoContato { get; set; } = EnumTipoRetornoContato.Whatsapp;

        [GridField("Status", Order = 4, EnumRender = EnumRenderType.IconDescription)]
        [FormField(Order = 31, Name = "Status", Section = "Retorno", Icon = "fas fa-info-circle", Type = EnumFieldType.Select)]
        public EnumStatusLead Status { get; set; } = EnumStatusLead.Pendente;

        [FormField(Order = 40, Name = "Contexto", Section = "Contexto", Icon = "fas fa-commenting", Type = EnumFieldType.TextArea, GridColumns = 1)]
        public string? Contexto { get; set; }

        [GridStatus(ShowInGrid = false)]
        public new bool Ativo { get; set; } = true;

    }
}