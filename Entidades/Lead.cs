using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Lead´s", Subtitle = "Gerencie os leads", Icon = "fas fa-truck", EnableAjaxSubmit = true)]
    public class Lead : BaseEntidadeDocumento
    {
        [GridField("Endereço", Order = 70, ShowInGrid = false)]
        [FormField(Order = 30, Name = "Endereço", Section = "Endereço", Icon = "fas fa-road", Type = EnumFieldType.Text, GridColumns = 3)]
        public string? Contexto { get; set; }

        [GridField("Endereço", Order = 70, ShowInGrid = false)]
        [FormField(Order = 30, Name = "Endereço", Section = "Endereço", Icon = "fas fa-road", Type = EnumFieldType.Text, GridColumns = 3)]
        public EnumTipoContato TipoContato { get; set; }
    }
}