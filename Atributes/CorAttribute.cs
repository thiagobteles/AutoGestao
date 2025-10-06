namespace AutoGestao.Atributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class CorAttribute(string cor) : Attribute
    {
        public string Cor { get; set; } = cor;
    }
}