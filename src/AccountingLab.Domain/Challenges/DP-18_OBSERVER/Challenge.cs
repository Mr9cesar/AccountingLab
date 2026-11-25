using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP18Observer;

public sealed record FinancialEvent(Guid EventId, string Kind, decimal Amount);
public interface IFinancialNotificationsChallenge
{
    void Subscribe(string subscriberId, Action<FinancialEvent> receive);
    void Unsubscribe(string subscriberId);
    IReadOnlyList<string> Publish(FinancialEvent financialEvent);
}
public sealed class FinancialNotificationsChallenge
    : IFinancialNotificationsChallenge
{
    private readonly Dictionary<string, Action<FinancialEvent>>
        _subscribers = new();

    public void Subscribe(
        string subscriberId,
        Action<FinancialEvent> receive)
    {
        if (!_subscribers.TryAdd(subscriberId, receive))
        {
            throw new ArgumentException(
                "El suscriptor ya existe.");
        }
    }

    public void Unsubscribe(string subscriberId)
    {
        _subscribers.Remove(subscriberId);
    }

    public IReadOnlyList<string> Publish(
        FinancialEvent financialEvent)
    {
        var recipients = new List<string>();

        foreach (var subscriber in _subscribers.ToArray())
        {
            subscriber.Value(financialEvent);
            recipients.Add(subscriber.Key);
        }

        return recipients;
    }
}

