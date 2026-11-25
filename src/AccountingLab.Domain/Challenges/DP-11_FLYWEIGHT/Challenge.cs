using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP11Flyweight;

public sealed record MovementClassificationInput(Guid Id, decimal Amount, string Code, string Label, string FictionalTaxScope);
public sealed record ClassificationMetadata(string Code, string Label, string FictionalTaxScope);
public sealed record ClassifiedMovement(Guid Id, decimal Amount, ClassificationMetadata Metadata);
public interface ISharedClassificationChallenge { IReadOnlyList<ClassifiedMovement> Prepare(IReadOnlyList<MovementClassificationInput> movements); }
public sealed class SharedClassificationChallenge : ISharedClassificationChallenge
{
    public IReadOnlyList<ClassifiedMovement> Prepare(
    IReadOnlyList<MovementClassificationInput> movements)
    {
        var factory = new ClassificationMetadataFactory();
        var result = new List<ClassifiedMovement>();

        foreach (var movement in movements)
        {
            var metadata = factory.GetMetadata(
                movement.Code,
                movement.Label,
                movement.FictionalTaxScope);

            result.Add(new ClassifiedMovement(
                movement.Id,
                movement.Amount,
                metadata));
        }

        return result;
    }
}

public sealed class ClassificationMetadataFactory
{
    private readonly Dictionary<
        (string Code, string Label, string Scope),
        ClassificationMetadata> _cache = new();

    public ClassificationMetadata GetMetadata(
        string code, string label, string scope)
    {
        var key = (code, label, scope);

        if (!_cache.TryGetValue(key, out var metadata))
        {
            metadata = new ClassificationMetadata(
                code, label, scope);

            _cache.Add(key, metadata);
        }

        return metadata;
    }
}
