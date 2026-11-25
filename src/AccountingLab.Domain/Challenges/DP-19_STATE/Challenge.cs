using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP19State;

public sealed record InvoiceLifecycleRequest(string CurrentState, string Action, decimal OutstandingAmount, DateOnly DueDate, DateOnly BusinessDate);
public sealed record InvoiceTransitionResult(string State, bool PaymentAllowed);
public interface IInvoiceLifecycleChallenge { InvoiceTransitionResult Transition(InvoiceLifecycleRequest request); }
public sealed class InvoiceLifecycleChallenge : IInvoiceLifecycleChallenge
{
    public InvoiceTransitionResult Transition(
    InvoiceLifecycleRequest request)
    {
        IInvoiceState state = request.CurrentState switch
        {
            "draft" => new DraftState(),
            "issued" => new IssuedState(),
            "overdue" => new OverdueState(),
            "paid" => new PaidState(),
            _ => throw new InvalidOperationException(
                "Estado desconocido.")
        };

        return state.Transition(request);
    }
}

public interface IInvoiceState
{
    InvoiceTransitionResult Transition(
        InvoiceLifecycleRequest request);
}

public sealed class DraftState : IInvoiceState
{
    public InvoiceTransitionResult Transition(
        InvoiceLifecycleRequest request)
    {
        if (request.Action == "emit")
            return new InvoiceTransitionResult("issued", true);

        throw new InvalidOperationException("Accion no permitida.");
    }
}

public sealed class IssuedState : IInvoiceState
{
    public InvoiceTransitionResult Transition(
        InvoiceLifecycleRequest request)
    {
        if (request.Action == "pay")
        {
            return request.OutstandingAmount == 0m
                ? new InvoiceTransitionResult("paid", false)
                : new InvoiceTransitionResult("issued", true);
        }

        if (request.Action == "mark-overdue")
        {
            return request.BusinessDate > request.DueDate
                ? new InvoiceTransitionResult("overdue", true)
                : new InvoiceTransitionResult("issued", true);
        }

        throw new InvalidOperationException("Accion no permitida.");
    }
}

public sealed class OverdueState : IInvoiceState
{
    public InvoiceTransitionResult Transition(
        InvoiceLifecycleRequest request)
    {
        if (request.Action == "pay")
        {
            return request.OutstandingAmount == 0m
                ? new InvoiceTransitionResult("paid", false)
                : new InvoiceTransitionResult("overdue", true);
        }

        throw new InvalidOperationException("Accion no permitida.");
    }
}

public sealed class PaidState : IInvoiceState
{
    public InvoiceTransitionResult Transition(
        InvoiceLifecycleRequest request)
    {
        throw new InvalidOperationException(
            "La factura ya se encuentra pagada.");
    }
}