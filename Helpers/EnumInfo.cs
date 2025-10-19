namespace AutoGestao.Helpers
{
    /// <summary>
    /// Informações sobre um Enum
    /// </summary>
    public class EnumInfo
    {
        public string Name { get; set; } = "";
        public string FullName { get; set; } = "";
        public bool IsEnum { get; set; }
        public List<EnumValueInfo> Values { get; set; } = [];
    }
}
