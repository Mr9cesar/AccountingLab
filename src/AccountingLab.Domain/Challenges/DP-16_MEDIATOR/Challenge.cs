using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP16Mediator;

public sealed record InvoiceProcessingRequest(Guid InvoiceId, decimal Amount, bool IsValid, IReadOnlyList<string> Participants);
public sealed record InvoiceProcessingResult(Guid InvoiceId, bool Published, IReadOnlyList<string> ParticipantsInvoked);
public interface IInvoiceCoordinationChallenge { InvoiceProcessingResult Process(InvoiceProcessingRequest request); }
public sealed class InvoiceCoordinationChallenge : IInvoiceCoordinationChallenge
{
    public InvoiceProcessingResult Process(InvoiceProcessingRequest request)
    {
        if (request.Participants.Distinct().Count() !=
            request.Participants.Count)
        {
            throw new ArgumentException(
                "Existen participantes duplicados.");
        }

        var invoked = new List<string>();

        foreach (var participant in request.Participants)
        {
            invoked.Add(participant);

            if (participant == "validacion" && !request.IsValid)
            {
                return new InvoiceProcessingResult(
                    request.InvoiceId,
                    false,
                    invoked.ToArray());
            }
        }

        return new InvoiceProcessingResult(
            request.InvoiceId,
            invoked.Contains("publicacion"),
            invoked.ToArray());
    }
}

