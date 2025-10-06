using AutoGestao.Atributes;
using System.Reflection;

namespace AutoGestao.Helpers
{
    public class PropertyContext(PropertyInfo property)
    {
        public PropertyInfo Property { get; } = property;

        public GridFieldAttribute? GridField { get; } = property.GetCustomAttribute<GridFieldAttribute>();

        public bool HasGridField => GridField != null;
    }
}
