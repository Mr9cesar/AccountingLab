using AccountingLab.Domain.Education;

namespace AccountingLab.Application.Challenges.DP10Facade;

public sealed record ClosingAccountBalance(string AccountCode, decimal Debit, decimal Credit);
public sealed record ClosePeriodRequest(Guid CompanyId, DateOnly From, DateOnly To, IReadOnlyList<ClosingAccountBalance> Balances, int UnpostedDocuments);
public sealed record PeriodClosingSummary(Guid CompanyId, DateOnly From, DateOnly To, bool Closed, decimal TotalDebit, decimal TotalCredit);
public interface IPeriodClosingChallenge { PeriodClosingSummary Close(ClosePeriodRequest request); }
public sealed class PeriodClosingChallenge : IPeriodClosingChallenge
{
    public PeriodClosingSummary Close(ClosePeriodRequest request)
    {
        var validator = new PeriodClosingValidator();
        var calculator = new ClosingBalanceCalculator();

        validator.ValidatePeriod(request);
        validator.ValidateDocuments(request.UnpostedDocuments);

        var totals = calculator.Calculate(request.Balances);

        validator.ValidateBalance(totals.Debit, totals.Credit);

        return new PeriodClosingSummary(
            request.CompanyId,
            request.From,
            request.To,
            true,
            totals.Debit,
            totals.Credit);
    }
}

public sealed class PeriodClosingValidator
{
    public void ValidatePeriod(ClosePeriodRequest request)
    {
        if (request.From > request.To)
            throw new ArgumentException("Periodo no válido.");
    }

    public void ValidateDocuments(int unpostedDocuments)
    {
        if (unpostedDocuments > 0)
            throw new InvalidOperationException(
                "Existen documentos pendientes de contabilizar.");
    }

    public void ValidateBalance(decimal debit, decimal credit)
    {
        if (debit != credit)
            throw new InvalidOperationException(
                "Los saldos contables no están equilibrados.");
    }
}

public sealed class ClosingBalanceCalculator
{
    public (decimal Debit, decimal Credit) Calculate(
        IReadOnlyList<ClosingAccountBalance> balances)
    {
        return (
            balances.Sum(account => account.Debit),
            balances.Sum(account => account.Credit));
    }
}
