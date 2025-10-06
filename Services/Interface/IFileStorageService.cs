namespace AutoGestao.Services.Interface
{
    public interface IFileStorageService
    {
        /// <summary>
        /// Faz upload de um arquivo para o MinIO
        /// </summary>
        /// <param name="file">Arquivo a ser enviado</param>
        /// <param name="entityName">Nome da entidade (ex: "Cliente", "Veiculo")</param>
        /// <param name="propertyName">Nome da propriedade do campo</param>
        /// <param name="idEmpresa">ID da empresa (para composição do bucket)</param>
        /// <param name="customBucket">Bucket customizado (opcional)</param>
        /// <returns>Caminho relativo do arquivo no MinIO</returns>
        Task<string> UploadFileAsync(IFormFile file, string entityName, string propertyName, long idEmpresa, string? customBucket = null);

        /// <summary>
        /// Obtém URL pré-assinada para download de arquivo
        /// </summary>
        /// <param name="filePath">Caminho do arquivo</param>
        /// <param name="entityName">Nome da entidade</param>
        /// <param name="idEmpresa">ID da empresa</param>
        /// <param name="customBucket">Bucket customizado (opcional)</param>
        /// <param name="expirySeconds">Tempo de expiração da URL em segundos</param>
        /// <returns>URL pré-assinada para download</returns>
        Task<string> GetDownloadUrlAsync(string filePath, string entityName, long idEmpresa, string? customBucket = null, int? expirySeconds = null);

        /// <summary>
        /// Faz download direto do arquivo
        /// </summary>
        /// <param name="filePath">Caminho do arquivo</param>
        /// <param name="entityName">Nome da entidade</param>
        /// <param name="idEmpresa">ID da empresa</param>
        /// <param name="customBucket">Bucket customizado (opcional)</param>
        /// <returns>Stream do arquivo</returns>
        Task<Stream> DownloadFileAsync(string filePath, string entityName, long idEmpresa, string? customBucket = null);

        /// <summary>
        /// Exclui um arquivo do MinIO
        /// </summary>
        /// <param name="filePath">Caminho do arquivo</param>
        /// <param name="entityName">Nome da entidade</param>
        /// <param name="idEmpresa">ID da empresa</param>
        /// <param name="customBucket">Bucket customizado (opcional)</param>
        Task<bool> DeleteFileAsync(string filePath, string entityName, long idEmpresa, string? customBucket = null);

        /// <summary>
        /// Verifica se um arquivo existe
        /// </summary>
        Task<bool> FileExistsAsync(string filePath, string entityName, long idEmpresa, string? customBucket = null);

        /// <summary>
        /// Lista todos os arquivos de uma propriedade
        /// </summary>
        Task<List<string>> ListFilesAsync(string entityName, string propertyName, long idEmpresa, string? customBucket = null);
    }
}