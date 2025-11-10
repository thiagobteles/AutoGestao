using FGT.Data;
using FGT.Entidades.Base;
using FGT.Enumerador.Gerais;
using FGT.Extensions;
using FGT.Models;
using FGT.Models.Auth;
using FGT.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FGT.Controllers.Base
{
    [Authorize(Roles = "Admin")]
    public class UsuarioController(ApplicationDbContext context, IUsuarioService usuarioService, IFileStorageService fileStorageService, ILogger<StandardGridController<Usuario>> logger)
        : StandardGridController<Usuario>(context, fileStorageService, logger)
    {
        private readonly IUsuarioService _usuarioService = usuarioService;

        protected override IQueryable<Usuario> GetBaseQuery()
        {
            // IMPORTANTE: Não aplicar filtro automático por EmpresaCliente para usuários
            // Queremos que admins possam gerenciar TODOS os usuários do sistema
            _context.CurrentEmpresaId = GetCurrentEmpresaId();

            List<string> listaIgnorada = ["CriadoPorUsuario", "AlteradoPorUsuario", "Empresa"];

            var query = _context.Set<Usuario>().AsQueryable();

            // Buscar propriedades virtuais (navigation properties)
            var virtualProperties = typeof(Usuario).GetProperties()
                .Where(p => p.GetGetMethod()?.IsVirtual == true
                    && !p.GetGetMethod()?.IsFinal == true
                    && p.PropertyType.IsClass
                    && p.PropertyType != typeof(string)
                    && !typeof(System.Collections.IEnumerable).IsAssignableFrom(p.PropertyType))
                .ToList();

            // Aplicar Include para navigation properties
            foreach (var prop in virtualProperties.Where(x => !listaIgnorada.Contains(x.Name)))
            {
                query = query.Include(prop.Name);
            }

            // NÃO aplicar filtro por EmpresaCliente aqui
            return query.OrderByDescending(x => x.Id);
        }

        protected override StandardGridViewModel ConfigureCustomGrid(StandardGridViewModel standardGridViewModel)
        {
            standardGridViewModel.Filters =
            [
                new()
                {
                    Name = "search",
                    DisplayName = "Busca Geral",
                    Type = EnumGridFilterType.Text,
                    Placeholder = "Nome, Email ou CPF..."
                },
                new()
                {
                    Name = "perfil",
                    DisplayName = "Perfil",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos os perfis...",
                    Options = EnumExtension.GetSelectListItems<EnumPerfilUsuario>(true)
                },
                new()
                {
                    Name = "ativo",
                    DisplayName = "Status",
                    Type = EnumGridFilterType.Select,
                    Placeholder = "Todos...",
                    Options =
                    [
                        new SelectListItem { Value = "true", Text = "✅ Ativos" },
                        new SelectListItem { Value = "false", Text = "⛔ Inativos" }
                    ]
                }
            ];

            return standardGridViewModel;
        }

        protected override IQueryable<Usuario> ApplyFilters(IQueryable<Usuario> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "search":
                        var searchTerm = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            query = ApplyTextFilter(query, searchTerm,
                                u => u.Nome,
                                u => u.Email,
                                u => u.Cpf);
                        }
                        break;

                    case "perfil":
                        query = ApplyEnumFilter(query, filters, filter.Key, u => u.Perfil);
                        break;

                    case "ativo":
                        if (bool.TryParse(filter.Value.ToString(), out var ativo))
                        {
                            query = query.Where(u => u.Ativo == ativo);
                        }
                        break;
                }
            }

            return query;
        }

        protected override async Task BeforeCreate(Usuario entity)
        {
            // Validar email único
            if (await _usuarioService.EmailExisteAsync(entity.Email))
            {
                ModelState.AddModelError(nameof(entity.Email), "Email já cadastrado");
            }

            // Validar senha - a senha vem na entidade, não em Request.Form
            var senha = entity.SenhaHash;
            var confirmarSenha = entity.ConfirmarSenha;

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

            // Se senha foi informada, alterar - a senha vem na entidade
            var novaSenha = entity.SenhaHash;
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
            TempData["NotificationScript"] = $"showSuccess('Usuário {EscapeJavaScript(entity.Nome)} criado com sucesso!')";
            return base.AfterCreate(entity);
        }

        protected override Task AfterUpdate(Usuario entity)
        {
            TempData["NotificationScript"] = $"showSuccess('Usuário {EscapeJavaScript(entity.Nome)} atualizado com sucesso!')";
            return base.AfterUpdate(entity);
        }

        protected override Task<bool> CanDelete(Usuario entity)
        {
            // Não permitir deletar o próprio usuário ou se for o único admin
            var usuarioLogadoId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            if (entity.Id == usuarioLogadoId)
            {
                return Task.FromResult(false);
            }

            if (entity.Perfil == EnumPerfilUsuario.Admin)
            {
                var totalAdmins = _context.Usuarios.Count(u => u.Perfil == EnumPerfilUsuario.Admin && u.Ativo);
                return Task.FromResult(totalAdmins > 1);
            }

            return Task.FromResult(true);
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
        [Route("Usuario/AlterarSenha")]
        [AllowAnonymous] // Permitir que usuários logados alterem sua própria senha
        public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaRequest request)
        {
            if (request.NovaSenha != request.ConfirmarSenha)
            {
                return Json(new
                {
                    sucesso = false,
                    mensagem = "Senhas não conferem",
                    script = "showError('As senhas informadas não conferem. Verifique e tente novamente.')"
                });
            }

            var resultado = await _usuarioService.AlterarSenhaAsync(request.UsuarioId, request.SenhaAtual, request.NovaSenha);
            if (resultado)
            {
                return Json(new
                {
                    sucesso = true,
                    mensagem = "Senha alterada com sucesso",
                    script = "showSuccess('Sua senha foi alterada com sucesso!')"
                });
            }

            return Json(new
            {
                sucesso = false,
                mensagem = "Senha atual incorreta",
                script = "showError('A senha atual informada está incorreta. Verifique e tente novamente.')"
            });
        }
    }
}