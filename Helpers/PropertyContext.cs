using FGT.Atributes;
using System.Reflection;

namespace FGT.Helpers
{
    public class PropertyContext(PropertyInfo property)
    {
        public PropertyInfo Property { get; } = property;

        public GridFieldAttribute? GridField { get; } = property.GetCustomAttribute<GridFieldAttribute>();

        public bool HasGridField => GridField != null;
    }
}
