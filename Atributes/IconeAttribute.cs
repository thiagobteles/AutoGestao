namespace AutoGestao.Atributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class IconeAttribute(string icone) : Attribute
    {
        public string Icone { get; set; } = icone;
    }
}