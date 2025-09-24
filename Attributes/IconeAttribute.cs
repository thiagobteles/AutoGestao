namespace AutoGestao.Atributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class IconeAttribute(string icone) : Attribute
    {
        public string Icone { get; } = icone;
    }
}
