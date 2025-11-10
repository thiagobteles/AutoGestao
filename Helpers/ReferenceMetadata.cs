namespace FGT.Helpers
{
    public class ReferenceMetadata
    {
        public ReferencePropertyInfo? TextProperty { get; set; }

        public List<ReferencePropertyInfo> SubtitleProperties { get; set; } = [];

        public List<ReferencePropertyInfo> SearchableProperties { get; set; } = [];
    }
}
