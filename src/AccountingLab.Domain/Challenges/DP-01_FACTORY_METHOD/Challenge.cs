using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP01FactoryMethod;

public sealed record VoucherCreationRequest(Guid CompanyId, string Kind, decimal Amount, string Currency);
public sealed record VoucherDraft(Guid Id, Guid CompanyId, string Kind, decimal Amount, string Currency);
public interface IVoucherCreationChallenge { VoucherDraft Create(VoucherCreationRequest request); }
public sealed class VoucherCreationChallenge : IVoucherCreationChallenge
{
    public VoucherDraft Create(VoucherCreationRequest request)
    {
        VoucherCreator creator = request.Kind switch
        {
            "ingreso" => new IncomeCreator(),
            "egreso" => new ExpenseCreator(),
            "ajuste" => new AdjustmentCreator(),
            "traslado" => new TransferCreator(),
            _ => throw new ArgumentException("Tipo de comprobante no válido.")
        };

        return creator.Create(request);
    }
}

public abstract class VoucherCreator
{
    public abstract VoucherDraft Create(VoucherCreationRequest request);
}

public sealed class IncomeCreator : VoucherCreator
{
    public override VoucherDraft Create(VoucherCreationRequest request)
        => new(Guid.NewGuid(), request.CompanyId,
            "ingreso", request.Amount, request.Currency);
}

public sealed class ExpenseCreator : VoucherCreator
{
    public override VoucherDraft Create(VoucherCreationRequest request)
        => new(Guid.NewGuid(), request.CompanyId,
            "egreso", request.Amount, request.Currency);
}

public sealed class AdjustmentCreator : VoucherCreator
{
    public override VoucherDraft Create(VoucherCreationRequest request)
        => new(Guid.NewGuid(), request.CompanyId,
            "ajuste", request.Amount, request.Currency);
}

public sealed class TransferCreator : VoucherCreator
{
    public override VoucherDraft Create(VoucherCreationRequest request)
        => new(Guid.NewGuid(), request.CompanyId,
            "traslado", request.Amount, request.Currency);
}

