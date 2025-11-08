using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Services
{
    public class UsuarioService(ApplicationDbContext context) : IUsuarioService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Usuario> CriarUsuarioAsync(Usuario usuario, string senha)
        {
            usuario.SenhaHash = AuthService.HashPassword(senha);
            usuario.DataCadastro = DateTime.UtcNow;
            usuario.DataAlteracao = DateTime.UtcNow;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario?> BuscarPorEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> EmailExisteAsync(string email, long? usuarioId = null)
        {
            var query = _context.Usuarios.Where(u => u.Email.ToLower() == email.ToLower());
            if (usuarioId.HasValue)
            {
                query = query.Where(u => u.Id != usuarioId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> AlterarSenhaAsync(long usuarioId, string senhaAtual, string novaSenha)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(senhaAtual, usuario.SenhaHash))
            {
                return false;
            }

            usuario.SenhaHash = AuthService.HashPassword(novaSenha);
            usuario.DataAlteracao = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}