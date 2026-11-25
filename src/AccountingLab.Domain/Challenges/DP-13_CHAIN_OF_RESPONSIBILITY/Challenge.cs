using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP13ChainOfResponsibility;

public sealed record PaymentApprovalRequest(decimal Amount, decimal AvailableFunds, bool PolicyAllows, decimal AuthorizationLimit, IReadOnlyList<string> Checks);
public sealed record PaymentApprovalDecision(bool Approved, string? RejectedBy, IReadOnlyList<string> VisitedChecks);
public interface IPaymentApprovalChallenge { PaymentApprovalDecision Evaluate(PaymentApprovalRequest request); }
public sealed class PaymentApprovalChallenge : IPaymentApprovalChallenge
{
    public PaymentApprovalDecision Evaluate(PaymentApprovalRequest request)
    {
        var visited = new List<string>();

        foreach (var check in request.Checks)
        {
            IPaymentApprovalHandler handler = check switch
            {
                "politica" => new PolicyApprovalHandler(),
                "fondos" => new FundsApprovalHandler(),
                "limite" => new LimitApprovalHandler(),
                _ => throw new ArgumentException("Validación desconocida.")
            };

            visited.Add(check);

            if (!handler.Validate(request))
            {
                return new PaymentApprovalDecision(
                    false, check, visited.ToArray());
            }
        }

        return new PaymentApprovalDecision(
            true, null, visited.ToArray());
    }
}

public interface IPaymentApprovalHandler
{
    bool Validate(PaymentApprovalRequest request);
}

public sealed class PolicyApprovalHandler : IPaymentApprovalHandler
{
    public bool Validate(PaymentApprovalRequest request)
    {
        return request.PolicyAllows;
    }
}

public sealed class FundsApprovalHandler : IPaymentApprovalHandler
{
    public bool Validate(PaymentApprovalRequest request)
    {
        return request.Amount <= request.AvailableFunds;
    }
}

public sealed class LimitApprovalHandler : IPaymentApprovalHandler
{
    public bool Validate(PaymentApprovalRequest request)
    {
        return request.Amount <= request.AuthorizationLimit;
    }
}

