using AutoGestao.Data;
using AutoGestao.Services;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework com PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IAuditCleanupService, AuditCleanupService>();

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

    await InicializarDadosPadrao(context, usuarioService);
}

app.Run();

static async Task InicializarDadosPadrao(ApplicationDbContext context, IUsuarioService usuarioService)
{
    await context.Database.MigrateAsync();

    if (!await context.Usuarios.AnyAsync(u => u.Perfil == AutoGestao.Enumerador.Gerais.EnumPerfilUsuario.Admin))
    {
        var adminUser = new AutoGestao.Entidades.Usuario
        {
            Nome = "Administrador",
            Email = "admin@autogestao.com",
            Perfil = AutoGestao.Enumerador.Gerais.EnumPerfilUsuario.Admin,
            Ativo = true
        };

        await usuarioService.CriarUsuarioAsync(adminUser, "admin123");

        Console.WriteLine("Usuário administrador criado:");
        Console.WriteLine("Email: admin@autogestao.com");
        Console.WriteLine("Senha: admin123");
    }
}