namespace AutoGestao.Models
{
    public class DeleteFileRequestModel
    {
        public string PropertyName { get; set; } = "";
        public string FilePath { get; set; } = "";
        public string? CustomBucket { get; set; }
    }
}
