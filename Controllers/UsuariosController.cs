using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Models;
using AutoGestao.Models.Auth;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AutoGestao.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsuariosController(ApplicationDbContext context, IUsuarioService usuarioService) : StandardGridController<Usuario>(context)
    {
        private readonly IUsuarioService _usuarioService = usuarioService;

        protected override async Task BeforeCreate(Usuario entity)
        {
            // Validar email único
            if (await _usuarioService.EmailExisteAsync(entity.Email))
            {
                ModelState.AddModelError(nameof(entity.Email), "Email já cadastrado");
            }

            // Validar senha
            var senha = Request.Form["SenhaHash"].ToString();
            var confirmarSenha = Request.Form["ConfirmarSenha"].ToString();

            if (string.IsNullOrEmpty(senha))
            {
                ModelState.AddModelError("SenhaHash", "Senha é obrigatória");
            }

            if (string.IsNullOrEmpty(confirmarSenha))
            {
                ModelState.AddModelError("ConfirmarSenha", "Confirmação de senha é obrigatória");
            }
            else if (confirmarSenha != senha)
            {
                ModelState.AddModelError("ConfirmarSenha", "Senhas não conferem");
            }

            if (ModelState.IsValid)
            {
                await _usuarioService.CriarUsuarioAsync(entity, senha);
            }
        }

        protected override async Task BeforeUpdate(Usuario entity)
        {
            entity.DataAlteracao = DateTime.UtcNow;

            // Validar email único (exceto o próprio usuário)
            if (await _usuarioService.EmailExisteAsync(entity.Email, entity.Id))
            {
                ModelState.AddModelError(nameof(entity.Email), "Email já cadastrado");
            }

            // Se senha foi informada, alterar
            var novaSenha = Request.Form["SenhaHash"].ToString();
            if (!string.IsNullOrEmpty(novaSenha))
            {
                entity.SenhaHash = Services.AuthService.HashPassword(novaSenha);
            }
            else
            {
                // Manter senha atual
                var usuarioAtual = await _context.Usuarios.AsNoTracking()
                    .FirstAsync(u => u.Id == entity.Id);
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
            var usuarioLogadoId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (entity.Id == usuarioLogadoId)
            {
                return false;
            }

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
                senhaField.Value = ""; // Nunca mostrar a senha atual
            }

            // Adicionar campo confirmar senha no Create e Edit
            if (action == "Create" || action == "Edit")
            {
                var confirmField = new FormFieldViewModel
                {
                    PropertyName = "ConfirmarSenha",
                    DisplayName = "Confirmar Senha",
                    Type = EnumFieldType.Password,
                    Required = action == "Create",
                    Section = "Dados Básicos",
                    Order = 4,
                    Icon = "fas fa-lock"
                };

                // Inserir após o campo senha
                var senhaIndex = fields.FindIndex(f => f.PropertyName == nameof(Usuario.SenhaHash));
                if (senhaIndex >= 0)
                {
                    fields.Insert(senhaIndex + 1, confirmField);
                }
                else
                {
                    fields.Add(confirmField);
                }
            }

            // Campo somente leitura em Details
            if (action == "Details")
            {
                var ultimoLoginField = fields.FirstOrDefault(f => f.PropertyName == nameof(Usuario.UltimoLogin));
                if (ultimoLoginField != null && entity.UltimoLogin.HasValue)
                {
                    ultimoLoginField.Value = entity.UltimoLogin.Value.ToString("dd/MM/yyyy HH:mm");
                }

                // Adicionar informações de roles
                var roles = GetRolesByPerfil(entity.Perfil.ToString());
                var rolesField = new FormFieldViewModel
                {
                    PropertyName = "RolesInfo",
                    DisplayName = "Permissões de Acesso",
                    Value = string.Join(", ", roles),
                    ReadOnly = true,
                    Section = "Informações do Sistema",
                    Icon = "fas fa-shield-alt"
                };
                fields.Add(rolesField);
            }

            // Remover campo ConfirmarSenha de entidade (não mapear)
            if (action == "Details")
            {
                fields.RemoveAll(f => f.PropertyName == nameof(Usuario.ConfirmarSenha));
            }
        }

        private static string[] GetRolesByPerfil(string perfil)
        {
            return perfil switch
            {
                "Admin" => ["Admin", "Gerente", "Vendedor", "Financeiro", "Visualizador"],
                "Gerente" => ["Gerente", "Vendedor", "Visualizador"],
                "Vendedor" => ["Vendedor", "Visualizador"],
                "Financeiro" => ["Financeiro", "Visualizador"],
                "Visualizador" => ["Visualizador"],
                _ => ["Visualizador"]
            };
        }

        [HttpPost]
        [Route("AlterarSenha")]
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaRequest request)
        {
            if (request.NovaSenha != request.ConfirmarSenha)
            {
                return Json(new { sucesso = false, mensagem = "Senhas não conferem" });
            }

            var resultado = await _usuarioService.AlterarSenhaAsync(request.UsuarioId, request.SenhaAtual, request.NovaSenha);
            return Json(new
            {
                sucesso = resultado,
                mensagem = resultado ? "Senha alterada com sucesso" : "Senha atual incorreta"
            });
        }
    }
}