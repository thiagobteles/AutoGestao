using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Services;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar cultura padrão para português do Brasil
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("pt-BR");

// Add Entity Framework com PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
        options.Cookie.Name = "AutoGestao.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();

// Registrar serviços
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IAuditCleanupService, AuditCleanupService>();
builder.Services.AddScoped<GenericReferenceService>();
builder.Services.AddHttpContextAccessor();

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
app.UseMiddleware<AutoGestao.Middleware.AuditMiddleware>();

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

    await InicializarDadosPadrao(context, usuarioService, empresaService);
}

app.Run();

static async Task InicializarDadosPadrao(ApplicationDbContext context, IUsuarioService usuarioService, IEmpresaService empresaService)
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
            Email = "admin@autogestao.com",
            Perfil = EnumPerfilUsuario.Admin,
            IdEmpresa = 1,
            Ativo = true
        };

        await usuarioService.CriarUsuarioAsync(adminUser, "admin123");

        Console.WriteLine("Usuário administrador criado:");
        Console.WriteLine("Email: admin@autogestao.com");
        Console.WriteLine("Senha: admin123");
    }
}