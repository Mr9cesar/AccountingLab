using AccountingLab.Domain.Education;

namespace AccountingLab.Application.Challenges.DP21TemplateMethod;

public sealed record BankStatementItem(string Id, string Reference, DateOnly Date, decimal Amount);
public sealed record ReconciliationLedgerItem(string Id, string Reference, DateOnly Date, decimal Amount);
public sealed record BankReconciliationRequest(string Mode, IReadOnlyList<BankStatementItem> BankItems, IReadOnlyList<ReconciliationLedgerItem> LedgerItems, decimal Tolerance);
public sealed record ReconciliationMatch(string BankItemId, string LedgerItemId, decimal Difference);
public sealed record BankReconciliationResult(IReadOnlyList<ReconciliationMatch> Matches, IReadOnlyList<string> UnmatchedBankItems, IReadOnlyList<string> UnmatchedLedgerItems);
public interface IBankReconciliationChallenge { BankReconciliationResult Reconcile(BankReconciliationRequest request); }
public sealed class BankReconciliationChallenge : IBankReconciliationChallenge
{
    public BankReconciliationResult Reconcile(
    BankReconciliationRequest request)
    {
        ReconciliationTemplate template = request.Mode switch
        {
            "exact" => new ExactReconciliation(),
            "tolerance" => new ToleranceReconciliation(),
            _ => throw new ArgumentException("Modo desconocido.")
        };

        return template.Reconcile(request);
    }
}

public abstract class ReconciliationTemplate
{
    public BankReconciliationResult Reconcile(
        BankReconciliationRequest request)
    {
        if (request.Tolerance < 0m)
            throw new ArgumentOutOfRangeException(
                nameof(request.Tolerance));

        var matches = new List<ReconciliationMatch>();
        var unmatchedBank = new List<string>();
        var remainingLedger = request.LedgerItems.ToList();

        foreach (var bank in request.BankItems)
        {
            var ledger = remainingLedger.FirstOrDefault(item =>
                item.Reference == bank.Reference &&
                item.Date == bank.Date &&
                AmountMatches(
                    Math.Abs(bank.Amount - item.Amount),
                    request.Tolerance));

            if (ledger == null)
            {
                unmatchedBank.Add(bank.Id);
                continue;
            }

            matches.Add(new ReconciliationMatch(
                bank.Id,
                ledger.Id,
                Math.Abs(bank.Amount - ledger.Amount)));

            remainingLedger.Remove(ledger);
        }

        return new BankReconciliationResult(
            matches,
            unmatchedBank,
            remainingLedger.Select(item => item.Id).ToArray());
    }

    protected abstract bool AmountMatches(
        decimal difference, decimal tolerance);
}

public sealed class ExactReconciliation
    : ReconciliationTemplate
{
    protected override bool AmountMatches(
        decimal difference, decimal tolerance)
    {
        return difference == 0m;
    }
}

public sealed class ToleranceReconciliation
    : ReconciliationTemplate
{
    protected override bool AmountMatches(
        decimal difference, decimal tolerance)
    {
        return difference <= tolerance;
    }
}

