using System.Reflection;

namespace FGT.Helpers
{
    public class ReferencePropertyInfo
    {
        public PropertyInfo Property { get; set; } = null!;

        public string? NavigationPath { get; set; }

        public string? Prefix { get; set; }

        public int Order { get; set; }

        public string? Format { get; set; }
    }
}
