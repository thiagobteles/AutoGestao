using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Tipo de Despesa", Subtitle = "Gerencie os tipos de despesas", Icon = "fas fa-tags", EnableAjaxSubmit = true)]
    public class DespesaTipo : BaseEntidade
    {
        [GridMain("Descrição")]
        [ReferenceSearchable]
        [ReferenceText]
        [FormField(Order = 1, Name = "Descrição", Section = "Dados Básicos", Icon = "fas fa-align-left", Type = EnumFieldType.Text, Required = true, GridColumns = 2, Placeholder = "Ex: Manutenção, IPVA, Licenciamento...")]
        public string Descricao { get; set; } = string.Empty;
    }
}