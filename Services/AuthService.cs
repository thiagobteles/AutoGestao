using AutoGestao.Data;
using AutoGestao.Entidades;
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
    public class AuthService(ApplicationDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger) : IAuthService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILogger<AuthService> _logger = logger;

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower() && u.Ativo);
                if (usuario == null || !VerifyPassword(request.Senha, usuario.SenhaHash))
                {
                    // Log de login falhado
                    if (usuario != null)
                    {
                        var auditService = _httpContextAccessor.HttpContext?.RequestServices.GetService<IAuditService>();
                        await auditService?.LogLoginAsync(usuario.Id, usuario.Nome, usuario.Email, false, "Senha incorreta");
                    }

                    return new LoginResponse
                    {
                        Sucesso = false,
                        Mensagem = "Email ou senha inválidos"
                    };
                }

                // Atualizar último login
                usuario.UltimoLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                // Log de login bem-sucedido
                var auditServiceSuccess = _httpContextAccessor.HttpContext?.RequestServices.GetService<IAuditService>();
                await auditServiceSuccess?.LogLoginAsync(usuario.Id, usuario.Nome, usuario.Email, true);

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

        public async Task<bool> ValidateTokenAsync(string token)
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
            // Implementar blacklist de tokens se necessário
            // Por enquanto, apenas log
            _logger.LogInformation("Usuário {UsuarioId} fez logout", usuarioId);
            await Task.CompletedTask;
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

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Nome),
                new(ClaimTypes.Email, usuario.Email),
                new("Perfil", usuario.Perfil.ToString())
            };

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
    }
}