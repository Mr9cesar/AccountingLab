using AccountingLab.Domain.Education;

namespace AccountingLab.Application.Challenges.DP12Proxy;

public sealed record SensitiveReportRequest(Guid CompanyId, bool MayRead, string ReportKey);
public sealed record SensitiveFinancialReport(Guid CompanyId, string ReportKey, decimal ConfidentialAmount);
public interface ISensitiveReportChallenge { SensitiveFinancialReport Get(SensitiveReportRequest request, Func<SensitiveFinancialReport> loadReport); }
public sealed class SensitiveReportChallenge : ISensitiveReportChallenge
{
    public SensitiveFinancialReport Get(
    SensitiveReportRequest request,
    Func<SensitiveFinancialReport> loadReport)
    {
        if (!request.MayRead)
        {
            throw new UnauthorizedAccessException(
                "No tiene permiso para consultar el reporte.");
        }

        var report = loadReport();

        if (report.CompanyId != request.CompanyId)
        {
            throw new InvalidOperationException(
                "El reporte pertenece a otra empresa.");
        }

        return report;
    }
}

