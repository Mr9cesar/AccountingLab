using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP02AbstractFactory;

public sealed record AccountingProfileRequest(string Profile, string FictionalRegime);
public sealed record AccountingFamilyMember(string Role, string CompatibilityKey);
public sealed record AccountingServiceFamily(string Profile, string FictionalRegime, IReadOnlyList<AccountingFamilyMember> Members);
public interface IAccountingProfileChallenge { AccountingServiceFamily CreateFamily(AccountingProfileRequest request); }
public sealed class AccountingProfileChallenge : IAccountingProfileChallenge
{
    public AccountingServiceFamily CreateFamily(AccountingProfileRequest request)
    {
        AccountingFamilyFactory factory = request.Profile switch
        {
            "comercial" => new CommercialFamilyFactory(),
            "servicios" => new ServicesFamilyFactory(),
            _ => throw new ArgumentException("Perfil contable no válido.")
        };

        return new AccountingServiceFamily(
            request.Profile,
            request.FictionalRegime,
            new[]
            {
                factory.CreateCalculation(request.FictionalRegime),
                factory.CreateValidation(request.FictionalRegime),
                factory.CreateReport(request.FictionalRegime)
            });
    }
}

public abstract class AccountingFamilyFactory
{
    protected abstract string Profile { get; }

    protected string GetKey(string regime)
        => $"{Profile}-{regime}";

    public AccountingFamilyMember CreateCalculation(string regime)
        => new("calculo", GetKey(regime));

    public AccountingFamilyMember CreateValidation(string regime)
        => new("validacion", GetKey(regime));

    public AccountingFamilyMember CreateReport(string regime)
        => new("reporte", GetKey(regime));
}

public sealed class CommercialFamilyFactory : AccountingFamilyFactory
{
    protected override string Profile => "comercial";
}

public sealed class ServicesFamilyFactory : AccountingFamilyFactory
{
    protected override string Profile => "servicios";
}

