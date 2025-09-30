using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Services
{
    public class EmpresaService(ApplicationDbContext context) : IEmpresaService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Empresa> CriarEmpresaAsync(Empresa empresa)
        {
            empresa.DataCadastro = DateTime.UtcNow;
            empresa.DataAlteracao = DateTime.UtcNow;

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();
            return empresa;
        }
    }
}