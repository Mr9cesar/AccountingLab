using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP14Command;

public sealed record AccountingOperationRequest(Guid CompanyId, Guid OperationId, string Kind, Guid TargetId, decimal Amount);
public sealed record AccountingOperationReceipt(Guid OperationId, Guid TargetId, string Kind, decimal AppliedAmount, decimal RemainingAmount, Guid LinkedJournalEntryId, bool Replayed);
public sealed record AccountingOperationContext(decimal OutstandingAmount, bool TargetIsPublished, IReadOnlyList<AccountingOperationReceipt> CompletedOperations);
public interface IIdentifiedOperationChallenge { AccountingOperationReceipt Execute(AccountingOperationRequest request, AccountingOperationContext context); }
public sealed class IdentifiedOperationChallenge : IIdentifiedOperationChallenge
{
    public AccountingOperationReceipt Execute(
    AccountingOperationRequest request,
    AccountingOperationContext context)
    {
        var previous = context.CompletedOperations
            .FirstOrDefault(x => x.OperationId == request.OperationId);

        if (previous != null)
            return previous with { Replayed = true };

        switch (request.Kind)
        {
            case "apply-payment":
                return new AccountingOperationReceipt(
                    request.OperationId,
                    request.TargetId,
                    request.Kind,
                    request.Amount,
                    context.OutstandingAmount - request.Amount,
                    Guid.NewGuid(),
                    false);

            case "reverse-voucher":
                if (!context.TargetIsPublished)
                    throw new InvalidOperationException(
                        "El comprobante no está publicado.");

                return new AccountingOperationReceipt(
                    request.OperationId,
                    request.TargetId,
                    request.Kind,
                    request.Amount,
                    context.OutstandingAmount,
                    Guid.NewGuid(),
                    false);

            default:
                throw new InvalidOperationException(
                    "Operación no reconocida.");
        }
    }
}

