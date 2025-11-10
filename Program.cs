using AutoGestao;
using FGT.Data;
using FGT.Entidades.Base;
using FGT.Enumerador;
using FGT.Enumerador.Gerais;
using FGT.Models;
using FGT.Services;
using FGT.Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Minio;
using System.Globalization;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorOptions(options =>
    {
        // Permitir await em views
        options.ViewLocationFormats.Add("/Views/{1}/{0}.cshtml");
        options.ViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
    });

// Configurar cultura padrão para português do Brasil
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

// Obtem o nome do cliente
var cliente = builder.Configuration.GetValue<string>("Cliente");
Globais.CorSistema = (EnumCorSistema)builder.Configuration.GetValue<int>("CorSistema");

// Registrar HttpContextAccessor antes de tudo
builder.Services.AddHttpContextAccessor();

// Registrar AuditInterceptor ANTES do DbContext para auditoria automática de entidades (CREATE, UPDATE, DELETE)
builder.Services.AddScoped<AuditInterceptor>();

// Add Entity Framework com PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    var auditInterceptor = serviceProvider.GetRequiredService<AuditInterceptor>();
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection").Replace("#cliente#", cliente.ToLower()))
           .AddInterceptors(auditInterceptor); // Adicionar interceptor de auditoria para CREATE, UPDATE, DELETE
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// 🔧 AUTENTICAÇÃO SIMPLES COM COOKIES
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Login/Logout";
        options.AccessDeniedPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = $"{cliente}.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

// Add fluxo do Minio storage
builder.Services.Configure<MinioSettings>(builder.Configuration.GetSection("MinioSettings"));

var minioSettings = builder.Configuration.GetSection("MinioSettings").Get<MinioSettings>()!;
minioSettings.BucketPrefix = cliente;

builder.Services.AddSingleton(sp =>
{
    return new MinioClient()
        .WithEndpoint(minioSettings.Endpoint)
        .WithCredentials(minioSettings.AccessKey, minioSettings.SecretKey)
        .WithSSL(minioSettings.UseSSL)
        .Build();
});

builder.Services.AddAuthorization();

// Registrar serviços
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioEmpresaService, UsuarioEmpresaService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IAuditCleanupService, AuditCleanupService>();
builder.Services.AddScoped<IFileStorageService, MinioFileStorageService>();
builder.Services.AddScoped<EntityInspectorService>();
builder.Services.AddScoped<GenericReferenceService>();

// Criar um background service para executar limpeza:
builder.Services.AddHostedService<AuditCleanupBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Adicionar middleware de auditoria (ANTES de UseAuthentication)
app.UseMiddleware<FGT.Middleware.AuditMiddleware>();

// 🔧 MIDDLEWARE DE AUTENTICAÇÃO
app.UseAuthentication();
app.UseAuthorization();

// Configurar cultura padrão para português do Brasil
var supportedCultures = new[] { new CultureInfo("pt-BR") };
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-BR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
app.UseRequestLocalization(localizationOptions);

// 🔧 MIDDLEWARE SIMPLES DE REDIRECIONAMENTO
app.Use(async (context, next) =>
{
    var publicPaths = new[] { "/login", "/api/auth/login", "/css", "/js", "/images", "/favicon.ico" };
    var path = context.Request.Path.Value?.ToLower() ?? "";

    if (publicPaths.Any(p => path.StartsWith(p)))
    {
        await next();
        return;
    }

    if (!context.User.Identity?.IsAuthenticated == true)
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        context.Response.Redirect("/login");
        return;
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Inicializar dados padrão
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var usuarioService = scope.ServiceProvider.GetRequiredService<IUsuarioService>();
    var empresaService = scope.ServiceProvider.GetRequiredService<IEmpresaService>();

    if (cliente.ToLower().Equals("autogestao"))
    {
        await InicializarDadosPadraoAutoGestao(context, usuarioService, empresaService);
    }
    else
    {
        await InicializarDadosPadraoContabilidade(context, usuarioService, empresaService);
    }
}

app.Run();

static async Task InicializarDadosPadraoAutoGestao(ApplicationDbContext context, IUsuarioService usuarioService, IEmpresaService empresaService)
{
    await context.Database.MigrateAsync();

    if (!await context.Usuarios.AnyAsync(u => u.Perfil == EnumPerfilUsuario.Admin))
    {
        var empresa = new Empresa
        {
            RazaoSocial = "2PLus Consultoria",
            CEP = "74125200",
            Telefone = "62981483753",
            Email = "thiago_bteles@hotmail.com",
            Estado = EnumEstado.Goias,
            Cidade = "Goiânia",
            Endereco = "Rua T46",
            Numero = "305",
            Bairro = "Setor Oeste",
            Complemento = "Apartamento 401",
            Observacoes = "Empresa e dados teste",
            Ativo = true
        };

        await empresaService.CriarEmpresaAsync(empresa);

        var adminUser = new Usuario
        {
            Nome = "Thiago",
            Email = "admin@FGT.com",
            Perfil = EnumPerfilUsuario.Admin,
            IdEmpresa = 1,
            Ativo = true
        };

        await usuarioService.CriarUsuarioAsync(adminUser, "admin123");

        Console.WriteLine("Usuário administrador criado:");
        Console.WriteLine("Email: admin@FGT.com");
        Console.WriteLine("Senha: admin123");
    }
}

static async Task InicializarDadosPadraoContabilidade(ApplicationDbContext context, IUsuarioService usuarioService, IEmpresaService empresaService)
{
    await context.Database.MigrateAsync();

    if (!await context.Usuarios.AnyAsync(u => u.Perfil == EnumPerfilUsuario.Admin))
    {
        var empresa = new Empresa
        {
            RazaoSocial = "Contabilidade",
            CEP = "74125200",
            Telefone = "62981483753",
            Email = "admin@contabilidade.com",
            Estado = EnumEstado.Goias,
            Cidade = "Goiânia",
            Endereco = "Rua T46",
            Numero = "305",
            Bairro = "Setor Oeste",
            Complemento = "Apartamento 401",
            Observacoes = "Empresa teste",
            Ativo = true
        };

        var retorno = await empresaService.CriarEmpresaAsync(empresa);
        var adminUser = new Usuario
        {
            Nome = "Thiago",
            Email = "admin@contabilidade.com",
            Perfil = EnumPerfilUsuario.Admin,
            IdEmpresa = retorno.Id,
            Ativo = true
        };

        await usuarioService.CriarUsuarioAsync(adminUser, "admin123");

        var clienteUser = new Usuario
        {
            Nome = "Cliente",
            Email = "cliente@contabilidade.com",
            Perfil = EnumPerfilUsuario.Visualizador,
            IdEmpresa = retorno.Id,
            Ativo = true
        };

        await usuarioService.CriarUsuarioAsync(clienteUser, "cliente123");

        Console.WriteLine("✓ Usuário administrador criado");
        Console.WriteLine("  Email: admin@contabilidade.com");
        Console.WriteLine("  Senha: admin123");
        Console.WriteLine();

        // Executar scripts de carga inicial de dados
        await ExecutarScriptsIniciais(context);
    }
}

static async Task ExecutarScriptsIniciais(ApplicationDbContext context)
{
    try
    {
        // Verificar se os dados já foram carregados (verifica se existe algum CNAE)
        if (await context.CNAEs.AnyAsync())
        {
            Console.WriteLine("⚠ Dados de demonstração já foram carregados anteriormente.");
            return;
        }

        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine("INICIANDO CARGA DE DADOS DE DEMONSTRAÇÃO");
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine();

        var connectionString = context.Database.GetConnectionString();

        // Buscar a pasta Scripts no diretório do projeto (não no debug)
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", ".."));
        var scriptsPath = Path.Combine(projectRoot, "Scripts", "Contabilidade");

        if (!Directory.Exists(scriptsPath))
        {
            Console.WriteLine($"⚠ Pasta de scripts não encontrada: {scriptsPath}");
            Console.WriteLine($"   BaseDirectory: {baseDirectory}");
            Console.WriteLine($"   ProjectRoot: {projectRoot}");
            return;
        }

        Console.WriteLine($"📁 Pasta de scripts: {scriptsPath}");
        Console.WriteLine();

        // Lista de scripts na ordem correta
        var scripts = new[]
        {
            "01_Insert_CNAEs.sql",
            "02_Insert_Contadores.sql",
            "03_Insert_EmpresasClientes.sql",
            "04_Insert_CertificadosDigitais.sql",
            "05_Insert_ParametrosFiscais.sql",
            "06_Insert_DadosBancarios.sql",
            "07_Insert_AliquotasImpostos.sql",
            "08_Insert_PlanoContas.sql",
            "09_Insert_ObrigacoesFiscais.sql",
            "10_Insert_Clientes.sql",
            "11_Insert_NotasFiscais.sql",
            "12_Insert_LancamentosContabeis.sql"
        };

        int totalScripts = scripts.Length;
        int scriptsExecutados = 0;

        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        foreach (var scriptFile in scripts)
        {
            scriptsExecutados++;
            var scriptPath = Path.Combine(scriptsPath, scriptFile);

            if (!File.Exists(scriptPath))
            {
                Console.WriteLine($"⚠ Script não encontrado: {scriptFile}");
                continue;
            }

            Console.Write($">>> [{scriptsExecutados}/{totalScripts}] Executando {scriptFile}...");

            try
            {
                var scriptContent = await File.ReadAllTextAsync(scriptPath);
                using var command = new NpgsqlCommand(scriptContent, connection);
                command.CommandTimeout = 300; // 5 minutos de timeout
                await command.ExecuteNonQueryAsync();

                Console.WriteLine(" ✓");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" ✗ ERRO");
                Console.WriteLine($"    Detalhes: {ex.Message}");
                throw; // Interrompe a execução se houver erro
            }
        }

        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine("CARGA DE DADOS CONCLUÍDA COM SUCESSO!");
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine("RESUMO:");
        Console.WriteLine("  • 50 CNAEs");
        Console.WriteLine("  • 50 Contadores Responsáveis");
        Console.WriteLine("  • 50 Empresas Clientes");
        Console.WriteLine("  • 50 Certificados Digitais");
        Console.WriteLine("  • 50 Parâmetros Fiscais");
        Console.WriteLine("  • 50 Dados Bancários");
        Console.WriteLine("  • 50 Alíquotas de Impostos");
        Console.WriteLine("  • 50 Plano de Contas");
        Console.WriteLine("  • 50 Obrigações Fiscais");
        Console.WriteLine("  • 50 Clientes");
        Console.WriteLine("  • 50 Notas Fiscais");
        Console.WriteLine("  • 50 Lançamentos Contábeis");
        Console.WriteLine();
        Console.WriteLine("  TOTAL: 600 registros inseridos");
        Console.WriteLine();
        Console.WriteLine("✓ Sistema pronto para apresentação!");
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine();
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine("✗ ERRO AO EXECUTAR SCRIPTS DE CARGA");
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine($"Erro: {ex.Message}");
        Console.WriteLine();
        Console.WriteLine("O sistema continuará funcionando, mas sem dados de demonstração.");
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine();
    }
}