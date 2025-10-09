using AutoGestao.Controllers.Base;
using AutoGestao.Entidades.Relatorio;
using AutoGestao.Models.Report;
using System.Text.Json;

namespace AutoGestao.Data
{
    public static class ReportTemplateSeeder
    {
        public static void SeedDefaultTemplates(ApplicationDbContext context)
        {
            if (context.ReportTemplates.Any())
            {
                return; // Já tem templates
            }

            var templates = new List<ReportTemplateEntity>
            {
                // Template Cliente
                new()
                {
                    Nome = "Cliente Completo",
                    TipoEntidade = "Cliente",
                    Descricao = "Relatório completo com todos os dados do cliente",
                    IsPadrao = true,
                    Ativo = true,
                    TemplateJson = JsonSerializer.Serialize(ReportController.GetClienteTemplate())
                },

                // Template Veículo
                new()
                {
                    Nome = "Veículo Detalhado",
                    TipoEntidade = "Veiculo",
                    Descricao = "Relatório com informações completas do veículo",
                    IsPadrao = true,
                    Ativo = true,
                    TemplateJson = JsonSerializer.Serialize(ReportController.GetVeiculoTemplate())
                },

                // Template Venda
                new()
                {
                    Nome = "Venda com Parcelas",
                    TipoEntidade = "Venda",
                    Descricao = "Relatório de venda incluindo detalhamento de parcelas",
                    IsPadrao = true,
                    Ativo = true,
                    TemplateJson = JsonSerializer.Serialize(ReportController.GetVendaTemplate())
                }
            };

            context.ReportTemplates.AddRange(templates);
            context.SaveChanges();
        }
    }
}