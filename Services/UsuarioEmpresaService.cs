using FGT.Data;
using FGT.Entidades;
using FGT.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FGT.Services
{
    public class UsuarioEmpresaService(ApplicationDbContext context) : IUsuarioEmpresaService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<long>> GetEmpresasDoUsuarioAsync(long idUsuario)
        {
            // Buscar empresas da tabela de relacionamento
            var empresasVinculadas = await _context.UsuarioEmpresaClientes
                .Where(ue => ue.IdUsuario == idUsuario && ue.Ativo)
                .Select(ue => ue.IdEmpresaCliente)
                .ToListAsync();

            // Se tiver empresa padrão (IdEmpresaCliente), incluir também
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            if (usuario?.IdEmpresaCliente.HasValue == true && !empresasVinculadas.Contains(usuario.IdEmpresaCliente.Value))
            {
                empresasVinculadas.Add(usuario.IdEmpresaCliente.Value);
            }

            return empresasVinculadas;
        }

        public async Task<bool> VincularEmpresaAsync(long idUsuario, long idEmpresaCliente)
        {
            // Verificar se já existe vínculo
            var vinculoExistente = await _context.UsuarioEmpresaClientes
                .FirstOrDefaultAsync(ue => ue.IdUsuario == idUsuario && ue.IdEmpresaCliente == idEmpresaCliente);

            if (vinculoExistente != null)
            {
                // Reativar se estava inativo
                if (!vinculoExistente.Ativo)
                {
                    vinculoExistente.Ativo = true;
                    await _context.SaveChangesAsync();
                }
                return true;
            }

            // Criar novo vínculo
            var novoVinculo = new UsuarioEmpresaCliente
            {
                IdUsuario = idUsuario,
                IdEmpresaCliente = idEmpresaCliente,
                DataVinculo = DateTime.UtcNow,
                Ativo = true
            };

            _context.UsuarioEmpresaClientes.Add(novoVinculo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DesvincularEmpresaAsync(long idUsuario, long idEmpresaCliente)
        {
            var vinculo = await _context.UsuarioEmpresaClientes
                .FirstOrDefaultAsync(ue => ue.IdUsuario == idUsuario && ue.IdEmpresaCliente == idEmpresaCliente);

            if (vinculo == null)
            {
                return false;
            }

            // Soft delete - apenas desativar
            vinculo.Ativo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TemAcessoEmpresaAsync(long idUsuario, long idEmpresaCliente)
        {
            // Verificar se tem vínculo ativo
            var temVinculo = await _context.UsuarioEmpresaClientes
                .AnyAsync(ue => ue.IdUsuario == idUsuario && ue.IdEmpresaCliente == idEmpresaCliente && ue.Ativo);

            if (temVinculo)
            {
                return true;
            }

            // Verificar se é a empresa padrão
            var usuario = await _context.Usuarios.FindAsync(idUsuario);
            return usuario?.IdEmpresaCliente == idEmpresaCliente;
        }
    }
}
