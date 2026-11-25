using AccountingLab.Domain.Education;
using System.Globalization;
using System.Text.Json;

namespace AccountingLab.Application.Challenges.DP07Bridge;

public sealed record ReportPresentationRequest(string ReportType, string Format, decimal Amount);
public sealed record RenderedFinancialReport(string ReportType, string Format, string ContentType, string Content);
public interface IFinancialRepresentationChallenge { RenderedFinancialReport Render(ReportPresentationRequest request); }
public sealed class FinancialRepresentationChallenge : IFinancialRepresentationChallenge
{
    public RenderedFinancialReport Render(ReportPresentationRequest request)
    {
        IReportFormat format = request.Format switch
        {
            "json" => new JsonReportFormat(),
            "csv" => new CsvReportFormat(),
            _ => throw new ArgumentException("Formato no válido.")
        };

        FinancialReportRepresentation report = request.ReportType switch
        {
            "balance" => new BalanceRepresentation(format),
            "resultados" => new ResultsRepresentation(format),
            _ => throw new ArgumentException("Tipo de reporte no válido.")
        };

        return report.Render(request.Amount);
    }
}

public interface IReportFormat
{
    string Format { get; }
    string ContentType { get; }
    string Render(string reportType, decimal amount);
}

public sealed class JsonReportFormat : IReportFormat
{
    public string Format => "json";
    public string ContentType => "application/json";

    public string Render(string reportType, decimal amount)
    {
        return JsonSerializer.Serialize(new
        {
            reportType,
            amount = amount.ToString("F4", CultureInfo.InvariantCulture)
        });
    }
}

public sealed class CsvReportFormat : IReportFormat
{
    public string Format => "csv";
    public string ContentType => "text/csv";

    public string Render(string reportType, decimal amount)
    {
        return $"reportType,amount\n{reportType},{amount.ToString("F4", CultureInfo.InvariantCulture)}";
    }
}

public abstract class FinancialReportRepresentation
{
    private readonly IReportFormat _format;

    protected FinancialReportRepresentation(IReportFormat format)
    {
        _format = format;
    }

    protected abstract string ReportType { get; }

    public RenderedFinancialReport Render(decimal amount)
    {
        return new RenderedFinancialReport(
            ReportType,
            _format.Format,
            _format.ContentType,
            _format.Render(ReportType, amount));
    }
}

public sealed class BalanceRepresentation : FinancialReportRepresentation
{
    public BalanceRepresentation(IReportFormat format) : base(format)
    {
    }

    protected override string ReportType => "balance";
}

public sealed class ResultsRepresentation : FinancialReportRepresentation
{
    public ResultsRepresentation(IReportFormat format) : base(format)
    {
    }

    protected override string ReportType => "resultados";
}

