using AutoGestao.Services;

namespace AutoGestao.Extensions
{
    public static class FileStorageServiceExtensions
    {
        /// <summary>
        /// Valida se o arquivo atende aos requisitos
        /// </summary>
        public static (bool IsValid, string ErrorMessage) ValidateFile(this IFormFile file, string allowedExtensions, int maxSizeMB)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "Arquivo inválido ou vazio");
            }

            // Validar extensão
            if (!string.IsNullOrEmpty(allowedExtensions))
            {
                var allowedExts = allowedExtensions
                    .Split(',')
                    .Select(e => e.Trim().ToLower())
                    .ToList();

                var fileExt = Path.GetExtension(file.FileName).ToLower().TrimStart('.');
                if (!allowedExts.Contains(fileExt))
                {
                    return (false, $"Extensão não permitida. Use: {allowedExtensions}");
                }
            }

            // Validar tamanho
            var maxSize = maxSizeMB * 1024 * 1024;
            if (file.Length > maxSize)
            {
                return (false, $"Arquivo muito grande. Máximo: {maxSizeMB}MB");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Copia múltiplos arquivos para o MinIO
        /// </summary>
        public static async Task<List<string>> UploadMultipleFilesAsync(
            this IFileStorageService service,
            IEnumerable<IFormFile> files,
            string entityName,
            string propertyName,
            long idEmpresa,
            string? customBucket = null)
        {
            var uploadedFiles = new List<string>();

            foreach (var file in files)
            {
                var filePath = await service.UploadFileAsync(file, entityName, propertyName, idEmpresa, customBucket);
                uploadedFiles.Add(filePath);
            }

            return uploadedFiles;
        }
    }
}