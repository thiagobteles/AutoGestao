using AutoGestao.Data;
using AutoGestao.Services.Interface;

namespace AutoGestao.Interfaces
{
    /// <summary>
    /// Interface base para entidades de processamento genéricas
    /// Entidades de processamento não são persistidas no banco,
    /// servem apenas para definir formulários de entrada de dados
    /// que disparam algum tipo de processamento
    /// </summary>
    /// <typeparam name="TResult">Tipo do resultado do processamento (pode ser uma entidade ou um resultado customizado)</typeparam>
    public interface IProcessingEntity<TResult>
    {
        /// <summary>
        /// Executa o processamento de forma assíncrona
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        /// <param name="fileStorageService">Serviço de armazenamento de arquivos</param>
        /// <param name="userId">ID do usuário que está executando o processamento</param>
        /// <param name="empresaId">ID da empresa do contexto atual</param>
        /// <returns>Resultado do processamento</returns>
        Task<ProcessingResult<TResult>> ProcessAsync(ApplicationDbContext context, IFileStorageService fileStorageService, long userId, long empresaId);
    }

    /// <summary>
    /// Resultado de um processamento
    /// </summary>
    /// <typeparam name="TResult">Tipo do resultado</typeparam>
    public class ProcessingResult<TResult>
    {
        /// <summary>
        /// Indica se o processamento foi bem-sucedido
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensagem de sucesso ou erro
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Dados resultantes do processamento
        /// </summary>
        public TResult? Data { get; set; }

        /// <summary>
        /// Lista de erros detalhados (se houver)
        /// </summary>
        public List<string> Errors { get; set; } = [];

        /// <summary>
        /// Lista de avisos (não impedem o sucesso)
        /// </summary>
        public List<string> Warnings { get; set; } = [];

        /// <summary>
        /// Dados adicionais do processamento (estatísticas, etc)
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = [];

        /// <summary>
        /// Cria um resultado de sucesso
        /// </summary>
        public static ProcessingResult<TResult> Ok(string message, TResult? data = default)
        {
            return new ProcessingResult<TResult>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Cria um resultado de erro
        /// </summary>
        public static ProcessingResult<TResult> Fail(string message, List<string>? errors = null)
        {
            return new ProcessingResult<TResult>
            {
                Success = false,
                Message = message,
                Errors = errors ?? []
            };
        }
    }
}
