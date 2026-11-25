using AccountingLab.Domain.Education;
using System.Text.Json;
using System.Globalization;

namespace AccountingLab.Infrastructure.Challenges.DP06Adapter;

public sealed record BankImportRequest(string Provider, string Payload);
public sealed record ImportedBankMovement(string ExternalId, DateOnly Date, string Description, decimal Debit, decimal Credit, string Currency);
public interface IBankImportChallenge { IReadOnlyList<ImportedBankMovement> Import(BankImportRequest request); }
public sealed class BankImportChallenge : IBankImportChallenge
{
    public IReadOnlyList<ImportedBankMovement> Import(BankImportRequest request)
    {
        IBankAdapter adapter = request.Provider switch
        {
            "BancoA" => new BancoAAdapter(),
            "BancoB" => new BancoBAdapter(),
            _ => throw new ArgumentException("Banco no válido.")
        };

        try
        {
            return adapter.Adapt(request.Payload);
        }
        catch (JsonException ex)
        {
            throw new FormatException("Formato bancario inválido.", ex);
        }
    }
}

public interface IBankAdapter
{
    IReadOnlyList<ImportedBankMovement> Adapt(string payload);
}

public sealed class BancoAAdapter : IBankAdapter
{
    public IReadOnlyList<ImportedBankMovement> Adapt(string payload)
    {
        using var document = JsonDocument.Parse(payload);

        return document.RootElement.EnumerateArray()
            .Select(item => new ImportedBankMovement(
                item.GetProperty("id").GetString()!,
                DateOnly.Parse(item.GetProperty("date").GetString()!),
                item.GetProperty("description").GetString()!,
                decimal.Parse(item.GetProperty("debit").GetString()!,
                    CultureInfo.InvariantCulture),
                decimal.Parse(item.GetProperty("credit").GetString()!,
                    CultureInfo.InvariantCulture),
                item.GetProperty("currency").GetString()!
            )).ToList();
    }
}

public sealed class BancoBAdapter : IBankAdapter
{
    public IReadOnlyList<ImportedBankMovement> Adapt(string payload)
    {
        using var document = JsonDocument.Parse(payload);

        return document.RootElement.EnumerateArray()
            .Select(item =>
            {
                decimal amount = decimal.Parse(
                    item.GetProperty("signedAmount").GetString()!,
                    CultureInfo.InvariantCulture);

                return new ImportedBankMovement(
                    item.GetProperty("reference").GetString()!,
                    DateOnly.Parse(item.GetProperty("bookedOn").GetString()!),
                    item.GetProperty("concept").GetString()!,
                    amount < 0 ? Math.Abs(amount) : 0m,
                    amount > 0 ? amount : 0m,
                    item.GetProperty("isoCurrency").GetString()!
                );
            }).ToList();
    }
}

