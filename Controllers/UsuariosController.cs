using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Models;
using AutoGestao.Services;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController(ApplicationDbContext context, IUsuarioService usuarioService) : StandardGridController<Usuario>(context)
    {
        private readonly IUsuarioService _usuarioService = usuarioService;

        protected override IQueryable<Usuario> GetBaseQuery()
        {
            return _context.Usuarios.AsQueryable();
        }

        protected override List<SelectListItem> GetSelectOptions(string propertyName)
        {
            return propertyName switch
            {
                nameof(Usuario.Perfil) => Enum.GetValues<EnumPerfilUsuario>()
                    .Select(e => new SelectListItem { Value = e.ToString(), Text = GetPerfilDisplayName(e) })
                    .ToList(),
                _ => base.GetSelectOptions(propertyName)
            };
        }

        protected override async Task BeforeCreate(Usuario entity)
        {
            // Validar email único
            if (await _usuarioService.EmailExisteAsync(entity.Email))
            {
                ModelState.AddModelError(nameof(entity.Email), "Email já cadastrado");
            }

            // Validar senha
            if (string.IsNullOrEmpty(entity.ConfirmarSenha))
            {
                ModelState.AddModelError(nameof(entity.ConfirmarSenha), "Confirmação de senha é obrigatória");
            }
            else if (entity.ConfirmarSenha != Request.Form["SenhaHash"])
            {
                ModelState.AddModelError(nameof(entity.ConfirmarSenha), "Senhas não conferem");
            }

            if (ModelState.IsValid)
            {
                await _usuarioService.CriarUsuarioAsync(entity, Request.Form["SenhaHash"]!);
            }
        }

        protected override async Task BeforeUpdate(Usuario entity)
        {
            entity.DataAlteracao = DateTime.Now;

            // Validar email único (exceto o próprio usuário)
            if (await _usuarioService.EmailExisteAsync(entity.Email, entity.Id))
            {
                ModelState.AddModelError(nameof(entity.Email), "Email já cadastrado");
            }

            // Se senha foi informada, alterar
            var novaSenha = Request.Form["SenhaHash"].ToString();
            if (!string.IsNullOrEmpty(novaSenha))
            {
                entity.SenhaHash = AuthService.HashPassword(novaSenha);
            }
            else
            {
                // Manter senha atual
                var usuarioAtual = await _context.Usuarios.AsNoTracking().FirstAsync(u => u.Id == entity.Id);
                entity.SenhaHash = usuarioAtual.SenhaHash;
            }
        }

        protected override Task AfterCreate(Usuario entity)
        {
            TempData["Success"] = $"Usuário {entity.Nome} criado com sucesso!";
            return base.AfterCreate(entity);

        }

        protected override Task AfterUpdate(Usuario entity)
        {
            TempData["Success"] = $"Usuário {entity.Nome} atualizado com sucesso!";
            return base.AfterUpdate(entity);
        }

        protected override bool CanDelete(Usuario entity)
        {
            // Não permitir deletar o próprio usuário ou se for o único admin
            var usuarioLogadoId = int.Parse(User.FindFirst("NameIdentifier")?.Value ?? "0");

            if (entity.Id == usuarioLogadoId)
                return false;

            if (entity.Perfil == EnumPerfilUsuario.Admin)
            {
                var totalAdmins = _context.Usuarios.Count(u => u.Perfil == EnumPerfilUsuario.Admin && u.Ativo);
                return totalAdmins > 1;
            }

            return true;
        }

        protected override void ConfigureFormFields(List<FormFieldViewModel> fields, Usuario entity, string action)
        {
            // Configurar campo de senha
            var senhaField = fields.FirstOrDefault(f => f.PropertyName == nameof(Usuario.SenhaHash));
            if (senhaField != null)
            {
                senhaField.DisplayName = action == "Create" ? "Senha" : "Nova Senha";
                senhaField.Required = action == "Create";
                senhaField.Placeholder = action == "Create" ? "Digite a senha" : "Deixe em branco para manter atual";
            }

            // Adicionar campo confirmar senha apenas no Create
            if (action == "Create")
            {
                var confirmField = fields.FirstOrDefault(f => f.PropertyName == nameof(Usuario.ConfirmarSenha));
                if (confirmField != null)
                {
                    confirmField.Required = true;
                }
            }
            else
            {
                // Remover campo confirmar senha do Edit
                fields.RemoveAll(f => f.PropertyName == nameof(Usuario.ConfirmarSenha));
            }

            // Campo somente leitura em Details
            if (action == "Details")
            {
                var ultimoLoginField = fields.FirstOrDefault(f => f.PropertyName == nameof(Usuario.UltimoLogin));
                if (ultimoLoginField != null && entity.UltimoLogin.HasValue)
                {
                    ultimoLoginField.Value = entity.UltimoLogin.Value.ToString("dd/MM/yyyy HH:mm");
                }
            }
        }

        private static string GetPerfilDisplayName(EnumPerfilUsuario perfil)
        {
            return perfil switch
            {
                EnumPerfilUsuario.Admin => "Administrador",
                EnumPerfilUsuario.Gerente => "Gerente",
                EnumPerfilUsuario.Vendedor => "Vendedor",
                EnumPerfilUsuario.Financeiro => "Financeiro",
                EnumPerfilUsuario.Visualizador => "Visualizador",
                _ => perfil.ToString()
            };
        }

        [HttpPost]
        [Route("AlterarSenha")]
        public async Task<IActionResult> AlterarSenha(int id, string senhaAtual, string novaSenha, string confirmarSenha)
        {
            if (novaSenha != confirmarSenha)
            {
                return Json(new { sucesso = false, mensagem = "Senhas não conferem" });
            }

            var resultado = await _usuarioService.AlterarSenhaAsync(id, senhaAtual, novaSenha);
            return Json(new
            {
                sucesso = resultado,
                mensagem = resultado ? "Senha alterada com sucesso" : "Senha atual incorreta"
            });
        }

        protected override StandardGridViewModel ConfigureGrid()
        {
            throw new NotImplementedException();
        }

        protected override IQueryable<Usuario> ApplyFilters(IQueryable<Usuario> query, Dictionary<string, object> filters)
        {
            throw new NotImplementedException();
        }

        protected override IQueryable<Usuario> ApplySort(IQueryable<Usuario> query, string orderBy, string orderDirection)
        {
            throw new NotImplementedException();
        }
    }
}