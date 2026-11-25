using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP17Memento;

public sealed class BudgetDraftLine
{
    public string AccountCode { get; set; } = "";
    public decimal Amount { get; set; }
}
public sealed class BudgetDraft
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public bool Approved { get; set; }
    public List<BudgetDraftLine> Lines { get; set; } = new();
}
public interface IBudgetSnapshot { }
public interface IBudgetHistoryChallenge
{
    IBudgetSnapshot Save(BudgetDraft draft);
    BudgetDraft Restore(BudgetDraft current, IBudgetSnapshot snapshot);
}
public sealed class BudgetHistoryChallenge : IBudgetHistoryChallenge
{
    public IBudgetSnapshot Save(BudgetDraft draft)
    {
        if (draft.Approved)
        {
            throw new InvalidOperationException(
                "No se puede guardar un presupuesto aprobado.");
        }

        return new BudgetSnapshot(Clone(draft));
    }

    public BudgetDraft Restore(
        BudgetDraft current,
        IBudgetSnapshot snapshot)
    {
        if (snapshot is not BudgetSnapshot saved)
        {
            throw new ArgumentException(
                "Snapshot no valido.");
        }

        return Clone(saved.Draft);
    }

    private static BudgetDraft Clone(BudgetDraft draft)
    {
        return new BudgetDraft
        {
            Id = draft.Id,
            Name = draft.Name,
            Approved = draft.Approved,
            Lines = draft.Lines.Select(line =>
                new BudgetDraftLine
                {
                    AccountCode = line.AccountCode,
                    Amount = line.Amount
                }).ToList()
        };
    }

    private sealed class BudgetSnapshot : IBudgetSnapshot
    {
        public BudgetDraft Draft { get; }

        public BudgetSnapshot(BudgetDraft draft)
        {
            Draft = draft;
        }
    }
}

