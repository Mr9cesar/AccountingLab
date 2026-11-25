using AccountingLab.Domain.Education;

namespace AccountingLab.Application.Challenges.DP15Iterator;

public sealed record LedgerRecord(string EntryCode, DateOnly Date, decimal Debit, decimal Credit);
public sealed record LedgerPage(IReadOnlyList<LedgerRecord> Records);
public interface ILedgerCursor { bool MoveNext(); LedgerRecord Current { get; } }
public interface ILedgerTraversalChallenge { ILedgerCursor Open(IReadOnlyList<LedgerPage> pages); }
public sealed class LedgerTraversalChallenge : ILedgerTraversalChallenge
{
    public ILedgerCursor Open(IReadOnlyList<LedgerPage> pages)
    {
        return new LedgerCursor(pages);
    }
}

public sealed class LedgerCursor : ILedgerCursor
{
    private readonly IEnumerator<LedgerRecord> _enumerator;
    private bool _hasCurrent;
    private bool _finished;

    public LedgerCursor(IReadOnlyList<LedgerPage> pages)
    {
        _enumerator = pages
            .SelectMany(page => page.Records)
            .GetEnumerator();
    }

    public LedgerRecord Current
    {
        get
        {
            if (!_hasCurrent)
                throw new InvalidOperationException(
                    "No existe un registro actual.");

            return _enumerator.Current;
        }
    }

    public bool MoveNext()
    {
        if (_finished)
            return false;

        _hasCurrent = _enumerator.MoveNext();

        if (!_hasCurrent)
        {
            _finished = true;
            _enumerator.Dispose();
        }

        return _hasCurrent;
    }
}

