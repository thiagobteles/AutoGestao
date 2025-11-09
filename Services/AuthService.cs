using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Base;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Models.Auth;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AutoGestao.Services
{
    public class AuthService(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger, IUsuarioEmpresaService usuarioEmpresaService) : IAuthService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IUsuarioEmpresaService _usuarioEmpresaService = usuarioEmpresaService;

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Ativo);

                if (usuario == null)
                {
                    return new LoginResponse
                    {
                        Sucesso = false,
                        Mensagem = "Email ou senha inválidos"
                    };
                }

                // Verificar se SenhaHash está definido
                if (string.IsNullOrEmpty(usuario.SenhaHash))
                {
                    _logger.LogError("Usuário {Email} não possui senha configurada", usuario.Email);
                    var auditService = _httpContextAccessor.HttpContext?.RequestServices.GetService<IAuditService>();
                    await auditService?.LogLoginAsync(usuario.Id, usuario.IdEmpresa, false, "Senha não configurada");

                    return new LoginResponse
                    {
                        Sucesso = false,
                        Mensagem = "Usuário sem senha configurada. Contate o administrador."
                    };
                }

                if (!VerifyPassword(request.Senha, usuario.SenhaHash))
                {
                    // Log de login falhado
                    var auditService = _httpContextAccessor.HttpContext?.RequestServices.GetService<IAuditService>();
                    await auditService?.LogLoginAsync(usuario.Id, usuario.IdEmpresa, false, "Senha incorreta");

                    return new LoginResponse
                    {
                        Sucesso = false,
                        Mensagem = "Email ou senha inválidos"
                    };
                }

                // Atualizar último login
                usuario.UltimoLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // DEBUG: Verificar se IdEmpresaCliente foi carregado
                _logger.LogInformation("🔍 LOGIN DEBUG - UsuarioId: {Id}, Nome: {Nome}, IdEmpresaCliente: {IdEmpresaCliente}",
                    usuario.Id, usuario.Nome, usuario.IdEmpresaCliente?.ToString() ?? "NULL");

                // Log de login bem-sucedido
                var auditServiceSuccess = _httpContextAccessor.HttpContext?.RequestServices.GetService<IAuditService>();
                await auditServiceSuccess?.LogLoginAsync(usuario.Id, usuario.IdEmpresa, true);

                // Buscar empresas vinculadas ao usuário
                var empresasVinculadas = await _usuarioEmpresaService.GetEmpresasDoUsuarioAsync(usuario.Id);

                _logger.LogInformation("🏢 Empresas vinculadas ao usuário {Id}: {Empresas}",
                    usuario.Id, string.Join(", ", empresasVinculadas));

                // Gerar token JWT
                var token = GenerateJwtToken(usuario);
                var roles = GetRolesByPerfil(usuario.Perfil.ToString());

                return new LoginResponse
                {
                    Sucesso = true,
                    Token = token,
                    Mensagem = "Login realizado com sucesso",
                    Usuario = new UsuarioInfo
                    {
                        Id = usuario.Id,
                        Nome = usuario.Nome,
                        Email = usuario.Email,
                        Perfil = usuario.Perfil.ToString(),
                        IdEmpresa = usuario.IdEmpresa,
                        IdEmpresaCliente = usuario.IdEmpresaCliente,
                        EmpresasVinculadas = empresasVinculadas,
                        Roles = roles
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar login para o email: {Email}", request.Email);
                return new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Erro interno do servidor"
                };
            }
        }

        public bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey não configurada"));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync(int usuarioId)
        {
            try
            {
                // Buscar usuário para obter IdEmpresa
                var usuario = await _context.Usuarios.FindAsync((long)usuarioId);

                // Registrar auditoria de logout
                var auditService = _httpContextAccessor.HttpContext?.RequestServices.GetService<IAuditService>();
                if (auditService != null && usuario != null)
                {
                    await auditService.LogLogoutAsync(usuario.Id, usuario.IdEmpresa);
                }

                _logger.LogInformation("Usuário {UsuarioId} fez logout", usuarioId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar logout do usuário {UsuarioId}", usuarioId);
            }
        }

        public string[] GetRolesByPerfil(string perfil)
        {
            return perfil switch
            {
                nameof(EnumPerfilUsuario.Admin) => ["Admin", "Gerente", "Vendedor", "Financeiro", "Visualizador"],
                nameof(EnumPerfilUsuario.Gerente) => ["Gerente", "Vendedor", "Visualizador"],
                nameof(EnumPerfilUsuario.Vendedor) => ["Vendedor", "Visualizador"],
                nameof(EnumPerfilUsuario.Financeiro) => ["Financeiro", "Visualizador"],
                nameof(EnumPerfilUsuario.Visualizador) => ["Visualizador"],
                _ => ["Visualizador"]
            };
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey não configurada"));
            var roles = GetRolesByPerfil(usuario.Perfil.ToString());

            _logger.LogInformation("🔐 GenerateJwtToken - UsuarioId: {Id}, IdEmpresaCliente: {IdEmpresaCliente}",
                usuario.Id, usuario.IdEmpresaCliente?.ToString() ?? "NULL");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Nome),
                new(ClaimTypes.Email, usuario.Email),
                new("Perfil", usuario.Perfil.ToString())
            };

            // Adicionar EmpresaClienteId se o usuário tiver vínculo
            if (usuario.IdEmpresaCliente.HasValue)
            {
                _logger.LogInformation("✅ Adicionando claim EmpresaClienteId: {Value}", usuario.IdEmpresaCliente.Value);
                claims.Add(new Claim("EmpresaClienteId", usuario.IdEmpresaCliente.Value.ToString()));
            }
            else
            {
                _logger.LogWarning("⚠️ IdEmpresaCliente é NULL - claim não será adicionada");
            }

            // Adicionar roles como claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8), // Token válido por 8 horas
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public async Task<List<EmpresaCliente>> ObterEmpresasPorIdsAsync(List<long> ids)
        {
            try
            {
                _logger.LogInformation("🔍 ObterEmpresasPorIdsAsync - Buscando empresas com IDs: {Ids}", string.Join(", ", ids));

                // IgnoreQueryFilters para permitir buscar empresas de qualquer tenant
                // (necessário para listar empresas vinculadas ao usuário)
                var empresas = await _context.EmpresasClientes
                    .IgnoreQueryFilters()
                    .Where(e => ids.Contains(e.Id))
                    .OrderBy(e => e.RazaoSocial)
                    .ToListAsync();

                _logger.LogInformation("✅ ObterEmpresasPorIdsAsync - Encontradas {Count} empresas", empresas.Count);

                return empresas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao buscar empresas pelos IDs: {Ids}", string.Join(", ", ids));
                return [];
            }
        }
    }
}