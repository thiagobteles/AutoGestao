namespace AutoGestao.Models
{
    public class MinioSettings
    {
        public string Endpoint { get; set; } = "31.97.172.113:9000";
        public string AccessKey { get; set; } = "admin";
        public string SecretKey { get; set; } = "Amigos25";
        public bool UseSSL { get; set; } = false;
        public int DefaultExpirySeconds { get; set; } = 3600;
        public string BucketPrefix { get; set; }
    }
}