using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP08Composite;

public sealed record AccountHierarchy(string Code, bool AllowsPosting, decimal OwnBalance, IReadOnlyList<AccountHierarchy> Children);
public interface IAccountTreeChallenge { decimal Total(AccountHierarchy account); }
public sealed class AccountTreeChallenge : IAccountTreeChallenge
{
    public decimal Total(AccountHierarchy account)
    {
        ArgumentNullException.ThrowIfNull(account);

        if (account.AllowsPosting)
        {
            if (account.Children.Count > 0)
                throw new ArgumentException(
                    "Una cuenta de movimiento no puede tener cuentas hijas.");

            return account.OwnBalance;
        }

        return account.Children.Sum(child => Total(child));
    }
}
