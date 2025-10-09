using AutoGestao.Entidades;
using AutoGestao.Models.Report;

namespace AutoGestao.Services.Interface
{
    public interface IReportService
    {
        string GenerateReportHtml<T>(T entity, ReportTemplate template) where T : BaseEntidade, new();

        ReportTemplate GetDefaultTemplate<T>() where T : BaseEntidade, new();

        List<ReportFieldInfo> GetReportFields<T>() where T : BaseEntidade, new();
    }
}