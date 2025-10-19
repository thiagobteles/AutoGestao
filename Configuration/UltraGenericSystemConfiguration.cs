using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Helpers;
using AutoGestao.Models;

namespace AutoGestao.Configuration
{
    /// <summary>
    /// Configuração para integração do Sistema Ultra-Genérico com o projeto atual
    /// Este arquivo permite migração gradual e coexistência dos dois sistemas
    /// </summary>
    public static class UltraGenericSystemConfiguration
    {
        /// <summary>
        /// Configura o sistema ultra-genérico no DI container
        /// </summary>
        public static IServiceCollection AddUltraGenericSystem(this IServiceCollection services)
        {
            // Registrar serviços específicos do sistema ultra-genérico
            services.AddScoped<IAutoFormService, AutoFormService>();
            services.AddScoped<IAutoGridService, AutoGridService>();

            // Configurar cache automático
            services.AddMemoryCache();
            services.AddSingleton<IEntityMetadataCacheService, EntityMetadataCacheService>();

            return services;
        }

        /// <summary>
        /// Inicialização automática do sistema (warm-up dos caches)
        /// </summary>
        public static async Task InitializeUltraGenericSystemAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("UltraGenericSystem");

            try
            {

                // Warm-up dos caches principais
                await Task.WhenAll(
                    AutoEnumHelper.WarmUpCachesAsync(),
                    WarmUpEntityCachesAsync(),
                    ValidateSystemIntegrityAsync(context, logger)
                );
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Aquece caches de todas as entidades principais
        /// </summary>
        private static async Task WarmUpEntityCachesAsync()
        {
            await Task.Run(() =>
            {
                // Entidades principais do sistema atual
                var entityTypes = new[]
                {
                    typeof(Veiculo),
                    typeof(Cliente),
                    typeof(Fornecedor),
                    typeof(Vendedor),
                    typeof(DespesaTipo),
                    typeof(VeiculoCor),
                    typeof(VeiculoFilial)
                };

                Parallel.ForEach(entityTypes, entityType =>
                {
                    try
                    {
                        var cacheMethod = typeof(GlobalEntityMetadataCache)
                            .GetMethod(nameof(GlobalEntityMetadataCache.GetCache))
                            ?.MakeGenericMethod(entityType);

                        cacheMethod?.Invoke(null, null);
                    }
                    catch (Exception ex)
                    {
                        // Log do erro mas não falha o warm-up
                        Console.WriteLine($"Erro ao fazer warm-up de {entityType.Name}: {ex.Message}");
                    }
                });
            });
        }

        /// <summary>
        /// Valida integridade do sistema
        /// </summary>
        private static async Task ValidateSystemIntegrityAsync(ApplicationDbContext context, ILogger logger)
        {
            var validations = new List<Task>
            {
                ValidateEntityConfigurationsAsync(logger),
                ValidateEnumConfigurationsAsync(logger),
                ValidateDatabaseConnectionAsync(context, logger)
            };

            await Task.WhenAll(validations);
        }

        private static async Task ValidateEntityConfigurationsAsync(ILogger logger)
        {
            await Task.Run(() =>
            {
                var entityTypes = new[] { typeof(Veiculo), typeof(Cliente), typeof(Fornecedor) };

                foreach (var entityType in entityTypes)
                {
                    try
                    {
                        // Usar reflexão para chamar o método genérico
                        var method = typeof(GlobalEntityMetadataCache)
                            .GetMethod(nameof(GlobalEntityMetadataCache.GetCache))
                            ?.MakeGenericMethod(entityType);

                        var cache = method?.Invoke(null, null);

                        if (cache != null)
                        {
                            var hasGridProperties = cache.GetType()
                                .GetProperty("GridProperties")
                                ?.GetValue(cache) is System.Collections.IList list && list.Count > 0;

                            if (!hasGridProperties)
                            {
                                logger.LogWarning("⚠️ Entidade {EntityType} não possui propriedades de grid configuradas", entityType.Name);
                            }
                            else
                            {
                                logger.LogDebug("✅ Entidade {EntityType} validada com sucesso", entityType.Name);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "❌ Erro ao validar entidade {EntityType}", entityType.Name);
                    }
                }
            });
        }

        private static async Task ValidateEnumConfigurationsAsync(ILogger logger)
        {
            await Task.Run(() =>
            {
                try
                {
                    var enumTypes = AutoEnumHelper.GetAllEnumTypes();
                    logger.LogInformation("📊 Encontrados {Count} enums no sistema", enumTypes.Count);

                    foreach (var enumType in enumTypes)
                    {
                        var options = AutoEnumHelper.GetEnumOptions(enumType);
                        if (options.Any())
                        {
                            logger.LogDebug("✅ Enum {EnumType} configurado com {Count} opções", enumType.Name, options.Count);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "❌ Erro ao validar configurações de enums");
                }
            });
        }

        private static async Task ValidateDatabaseConnectionAsync(ApplicationDbContext context, ILogger logger)
        {
            try
            {
                await context.Database.CanConnectAsync();
                logger.LogDebug("✅ Conexão com banco de dados validada");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Erro de conexão com banco de dados");
                throw;
            }
        }
    }

    /// <summary>
    /// Serviços específicos do sistema ultra-genérico
    /// </summary>
    public interface IAutoFormService
    {
        Task<object> BuildFormViewModelAsync<T>(T entity, string action) where T : BaseEntidade;
        Task<ValidationResult> ValidateEntityAsync<T>(T entity, string action) where T : BaseEntidade;
    }

    public interface IAutoGridService
    {
        Task<StandardGridViewModel> BuildGridViewModelAsync<T>() where T : BaseEntidade;
        Task<object> ApplyFiltersAsync<T>(IQueryable<T> query, Dictionary<string, object> filters) where T : BaseEntidade;
    }

    public interface IEntityMetadataCacheService
    {
        EntityMetadataCache<T> GetCache<T>() where T : class;
        void ClearCache<T>() where T : class;
        Task WarmUpCacheAsync<T>() where T : class;
    }

    // Implementações básicas
    public class AutoFormService : IAutoFormService
    {
        public async Task<object> BuildFormViewModelAsync<T>(T entity, string action) where T : BaseEntidade
        {
            // Implementação será movida do UltraGenericController
            return new { message = "Implementação em desenvolvimento" };
        }

        public async Task<ValidationResult> ValidateEntityAsync<T>(T entity, string action) where T : BaseEntidade
        {
            // Implementação de validação
            return new ValidationResult { IsValid = true };
        }
    }

    public class AutoGridService : IAutoGridService
    {
        public async Task<StandardGridViewModel> BuildGridViewModelAsync<T>() where T : BaseEntidade
        {
            // Implementação será movida do UltraGenericController
            return new StandardGridViewModel();
        }

        public async Task<object> ApplyFiltersAsync<T>(IQueryable<T> query, Dictionary<string, object> filters) where T : BaseEntidade
        {
            // Implementação de filtros automáticos
            return query;
        }
    }

    public class EntityMetadataCacheService : IEntityMetadataCacheService
    {
        public EntityMetadataCache<T> GetCache<T>() where T : class
        {
            return GlobalEntityMetadataCache.GetCache<T>();
        }

        public void ClearCache<T>() where T : class
        {
            GlobalEntityMetadataCache.ClearCache<T>();
        }

        public async Task WarmUpCacheAsync<T>() where T : class
        {
            await Task.Run(() => GlobalEntityMetadataCache.GetCache<T>());
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = [];
        public string Message { get; set; } = "";
    }
}

/// <summary>
/// Extensão para Program.cs/Startup.cs
/// </summary>
namespace AutoGestao.Configuration
{
    public static class ProgramExtensions
    {
        /// <summary>
        /// Configuração completa para Program.cs
        /// </summary>
        public static async Task<WebApplication> ConfigureUltraGenericSystemAsync(this WebApplication app)
        {
            // Middleware para debugging em desenvolvimento
            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<UltraGenericDebugMiddleware>();
            }

            // Inicializar sistema ultra-genérico
            await UltraGenericSystemConfiguration.InitializeUltraGenericSystemAsync(app.Services);

            // Configurar routes automáticas se necessário
            ConfigureAutoRoutes(app);

            return app;
        }

        private static void ConfigureAutoRoutes(WebApplication app)
        {
            // Rotas automáticas para APIs AJAX do sistema ultra-genérico
            app.MapControllerRoute(
                name: "auto-search-reference",
                pattern: "{controller}/SearchReference",
                defaults: new { action = "SearchReference" }
            );

            app.MapControllerRoute(
                name: "auto-upload-file",
                pattern: "{controller}/UploadFile",
                defaults: new { action = "UploadFile" }
            );

            app.MapControllerRoute(
                name: "auto-entity-config",
                pattern: "{controller}/EntityConfig",
                defaults: new { action = "EntityConfig" }
            );
        }
    }

    /// <summary>
    /// Middleware para debugging do sistema ultra-genérico
    /// </summary>
    public class UltraGenericDebugMiddleware(RequestDelegate next, ILogger<UltraGenericDebugMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<UltraGenericDebugMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            // Debug de requests para controllers ultra-genéricos
            if (context.Request.Path.Value?.Contains("EntityConfig") == true)
            {
                _logger.LogInformation("🔍 Debug request: {Path}", context.Request.Path);
            }

            await _next(context);
        }
    }
}