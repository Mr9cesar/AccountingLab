using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP20Strategy;

public sealed record FictionalTaxRule(string Name, string OperationKind, DateOnly ValidFrom, DateOnly ValidTo, string CalculationKind, decimal Rate, decimal FixedAmount);
public sealed record TaxCalculationRequest(decimal BaseAmount, string OperationKind, DateOnly AccountingDate, IReadOnlyList<FictionalTaxRule> Rules);
public sealed record TaxCalculationResult(decimal BaseAmount, decimal AppliedRate, decimal TaxAmount, string RuleName);
public interface IConfigurableTaxChallenge { TaxCalculationResult Calculate(TaxCalculationRequest request); }
public sealed class ConfigurableTaxChallenge : IConfigurableTaxChallenge
{
    public TaxCalculationResult Calculate(
    TaxCalculationRequest request)
    {
        var rule = request.Rules.FirstOrDefault(r =>
            r.OperationKind == request.OperationKind &&
            request.AccountingDate >= r.ValidFrom &&
            request.AccountingDate <= r.ValidTo);

        if (rule == null)
        {
            throw new InvalidOperationException(
                "No existe una regla tributaria vigente.");
        }

        ITaxCalculationStrategy strategy = rule.CalculationKind switch
        {
            "percentage" => new PercentageTaxStrategy(),
            "fixed" => new FixedTaxStrategy(),
            _ => throw new InvalidOperationException(
                "Estrategia desconocida.")
        };

        var tax = strategy.Calculate(request.BaseAmount, rule);

        tax = Math.Round(tax, 4, MidpointRounding.AwayFromZero);

        return new TaxCalculationResult(
            request.BaseAmount,
            rule.Rate,
            tax,
            rule.Name);
    }
}

public interface ITaxCalculationStrategy
{
    decimal Calculate(decimal baseAmount, FictionalTaxRule rule);
}

public sealed class PercentageTaxStrategy : ITaxCalculationStrategy
{
    public decimal Calculate(
        decimal baseAmount, FictionalTaxRule rule)
    {
        return baseAmount * rule.Rate;
    }
}

public sealed class FixedTaxStrategy : ITaxCalculationStrategy
{
    public decimal Calculate(
        decimal baseAmount, FictionalTaxRule rule)
    {
        return rule.FixedAmount;
    }
}