using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using Microsoft.Extensions.Options;
using AutoGestao.Models;
using System.Security.Claims;
using Minio.ApiEndpoints;
using AutoGestao.Services.Interface;

namespace AutoGestao.Services
{
    public class MinioFileStorageService(IMinioClient minioClient, IOptions<MinioSettings> settings, ILogger<MinioFileStorageService> logger) : IFileStorageService
    {
        private readonly IMinioClient _minioClient = minioClient;
        private readonly MinioSettings _settings = settings.Value;
        private readonly ILogger<MinioFileStorageService> _logger = logger;

        public async Task<string> UploadFileAsync(
            IFormFile file,
            string entityName,
            string propertyName,
            long idEmpresa,
            string? customBucket = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("Arquivo inválido");
                }

                var bucketName = customBucket ?? GetBucketName(entityName, idEmpresa);
                await EnsureBucketExistsAsync(bucketName);

                // Gerar nome único para o arquivo
                var fileExtension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{propertyName}/{Guid.NewGuid()}{fileExtension}";
                var contentType = file.ContentType ?? GetContentType(fileExtension);

                // Upload para o MinIO
                using var stream = file.OpenReadStream();
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(uniqueFileName)
                    .WithStreamData(stream)
                    .WithObjectSize(file.Length)
                    .WithContentType(contentType);

                await _minioClient.PutObjectAsync(putObjectArgs);

                _logger.LogInformation(
                    "Arquivo enviado com sucesso: {BucketName}/{FileName}",
                    bucketName,
                    uniqueFileName);

                return uniqueFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer upload do arquivo");
                throw;
            }
        }

        public async Task<Stream> DownloadFileAsync(string filePath, string entityName, long idEmpresa, string? customBucket = null)
        {
            try
            {
                var bucketName = customBucket ?? GetBucketName(entityName, idEmpresa);
                var memoryStream = new MemoryStream();

                var getObjectArgs = new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(filePath)
                    .WithCallbackStream(stream =>
                    {
                        stream.CopyTo(memoryStream);
                    });

                await _minioClient.GetObjectAsync(getObjectArgs);
                memoryStream.Position = 0;

                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer download do arquivo");
                throw;
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath, string entityName, long idEmpresa, string? customBucket = null)
        {
            try
            {
                var bucketName = customBucket ?? GetBucketName(entityName, idEmpresa);

                var removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(filePath);

                await _minioClient.RemoveObjectAsync(removeObjectArgs);

                _logger.LogInformation(
                    "Arquivo excluído com sucesso: {BucketName}/{FilePath}",
                    bucketName,
                    filePath);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir arquivo");
                return false;
            }
        }

        public async Task<string> GetDownloadUrlAsync(string filePath, string entityName, long idEmpresa, string? customBucket = null, int? expirySeconds = null)
        {
            try
            {
                var bucketName = customBucket ?? GetBucketName(entityName, idEmpresa);
                var expiry = expirySeconds ?? _settings.DefaultExpirySeconds;

                _logger.LogInformation($"[MINIO] Gerando URL para: Bucket={bucketName}, File={filePath}, Expiry={expiry}s");

                // Verificar se o arquivo existe
                var exists = await FileExistsAsync(filePath, entityName, idEmpresa, customBucket);
                if (!exists)
                {
                    _logger.LogWarning($"[MINIO] Arquivo não encontrado: {bucketName}/{filePath}");
                    throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");
                }

                var presignedGetObjectArgs = new PresignedGetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(filePath)
                    .WithExpiry(expiry);

                var url = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);

                _logger.LogInformation($"[MINIO] URL gerada com sucesso: {url}");

                return url;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[MINIO] Erro ao gerar URL de download");
                throw;
            }
        }

        public async Task<bool> FileExistsAsync(string filePath, string entityName, long idEmpresa, string? customBucket = null)
        {
            try
            {
                var bucketName = customBucket ?? GetBucketName(entityName, idEmpresa);

                var statObjectArgs = new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(filePath);

                await _minioClient.StatObjectAsync(statObjectArgs);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<string>> ListFilesAsync(string entityName, string propertyName, long idEmpresa, string? customBucket = null)
        {
            try
            {
                var bucketName = customBucket ?? GetBucketName(entityName, idEmpresa);
                var files = new List<string>();

                var listObjectsArgs = new ListObjectsArgs()
                    .WithBucket(bucketName)
                    .WithPrefix($"{propertyName}/")
                    .WithRecursive(false);

                var observable = _minioClient.ListObjectsEnumAsync(listObjectsArgs);

                await foreach (var item in observable)
                {
                    files.Add(item.Key);
                }

                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar arquivos");
                return [];
            }
        }

        private async Task EnsureBucketExistsAsync(string bucketName)
        {
            try
            {
                var beArgs = new BucketExistsArgs().WithBucket(bucketName);
                var found = await _minioClient.BucketExistsAsync(beArgs);

                if (!found)
                {
                    var mbArgs = new MakeBucketArgs().WithBucket(bucketName);
                    await _minioClient.MakeBucketAsync(mbArgs);

                    _logger.LogInformation("Bucket criado: {BucketName}", bucketName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar/criar bucket");
                throw;
            }
        }

        private string GetBucketName(string entityName, long idEmpresa)
        {
            var entityNameLower = entityName.ToLowerInvariant();
            var bucketName = $"{_settings.BucketPrefix}-{idEmpresa}-{entityNameLower}";

            // MinIO bucket names devem seguir regras DNS
            // Remover caracteres especiais e limitar tamanho
            bucketName = new string([.. bucketName.Where(c => char.IsLetterOrDigit(c) || c == '-')]);

            if (bucketName.Length > 63)
            {
                bucketName = bucketName.Substring(0, 63);
            }

            return bucketName.ToLowerInvariant();
        }

        private static string GetContentType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                ".txt" => "text/plain",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                _ => "application/octet-stream"
            };
        }
    }
}
