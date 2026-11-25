using AccountingLab.Domain.Education;

namespace AccountingLab.Application.Challenges.DP03Builder;

public sealed record FinancialReportRequest(string Title, DateOnly From, DateOnly To, IReadOnlyList<string> Sections, IReadOnlyList<string> Annexes);
public sealed record FinancialReport(string Title, DateOnly From, DateOnly To, IReadOnlyList<string> Sections, IReadOnlyList<string> Annexes);
public interface IReportAssemblyChallenge { FinancialReport Build(FinancialReportRequest request); }
public sealed class ReportAssemblyChallenge : IReportAssemblyChallenge
{
    public FinancialReport Build(FinancialReportRequest request)
    {
        return new FinancialReportBuilder()
            .SetTitle(request.Title)
            .SetDates(request.From, request.To)
            .SetSections(request.Sections)
            .SetAnnexes(request.Annexes)
            .Build();
    }
}

public sealed class FinancialReportBuilder
{
    private string _title = string.Empty;
    private DateOnly _from;
    private DateOnly _to;
    private IReadOnlyList<string> _sections = Array.Empty<string>();
    private IReadOnlyList<string> _annexes = Array.Empty<string>();

    public FinancialReportBuilder SetTitle(string title)
    {
        _title = title;
        return this;
    }

    public FinancialReportBuilder SetDates(DateOnly from, DateOnly to)
    {
        if (from > to)
            throw new ArgumentException("Intervalo de fechas no válido.");

        _from = from;
        _to = to;
        return this;
    }

    public FinancialReportBuilder SetSections(IReadOnlyList<string> sections)
    {
        _sections = sections.ToArray();
        return this;
    }

    public FinancialReportBuilder SetAnnexes(IReadOnlyList<string> annexes)
    {
        _annexes = annexes.ToArray();
        return this;
    }

    public FinancialReport Build()
    {
        return new FinancialReport(
            _title, _from, _to, _sections, _annexes);
    }
}

