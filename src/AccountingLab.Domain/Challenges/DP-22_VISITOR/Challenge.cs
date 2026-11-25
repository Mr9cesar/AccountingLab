using AccountingLab.Domain.Education;

namespace AccountingLab.Domain.Challenges.DP22Visitor;

public interface IAccountingAnalysisDocument { Guid Id { get; } }
public sealed record InvoiceAnalysisDocument(Guid Id, decimal NetAmount, decimal TaxAmount) : IAccountingAnalysisDocument;
public sealed record PaymentAnalysisDocument(Guid Id, decimal Amount) : IAccountingAnalysisDocument;
public sealed record AnalysisPostingLine(decimal Debit, decimal Credit);
public sealed record JournalAnalysisDocument(Guid Id, IReadOnlyList<AnalysisPostingLine> Lines) : IAccountingAnalysisDocument;
public sealed record DocumentAnalysisRequest(string View, IReadOnlyList<IAccountingAnalysisDocument> Documents);
public sealed record DocumentAnalysisResult(decimal TotalAmount, int DocumentCount, IReadOnlyDictionary<string, decimal> ByKind);
public interface IDocumentAnalysisChallenge { DocumentAnalysisResult Analyze(DocumentAnalysisRequest request); }
public sealed class DocumentAnalysisChallenge : IDocumentAnalysisChallenge
{
    public DocumentAnalysisResult Analyze(
    DocumentAnalysisRequest request)
    {
        IAccountingDocumentVisitor visitor = request.View switch
        {
            "gross" => new GrossAnalysisVisitor(),
            "count" => new CountAnalysisVisitor(),
            _ => throw new ArgumentException("Vista desconocida.")
        };

        var byKind = new Dictionary<string, decimal>();

        foreach (var document in request.Documents)
        {
            var result = document switch
            {
                InvoiceAnalysisDocument invoice =>
                    ("invoice", visitor.Visit(invoice)),

                PaymentAnalysisDocument payment =>
                    ("payment", visitor.Visit(payment)),

                JournalAnalysisDocument journal =>
                    ("journal", visitor.Visit(journal)),

                _ => throw new ArgumentException(
                    "Documento desconocido.")
            };

            byKind.TryGetValue(result.Item1, out var current);
            byKind[result.Item1] = current + result.Item2;
        }

        return new DocumentAnalysisResult(
            byKind.Values.Sum(),
            request.Documents.Count,
            byKind);
    }
}

public interface IAccountingDocumentVisitor
{
    decimal Visit(InvoiceAnalysisDocument document);
    decimal Visit(PaymentAnalysisDocument document);
    decimal Visit(JournalAnalysisDocument document);
}

public sealed class GrossAnalysisVisitor
    : IAccountingDocumentVisitor
{
    public decimal Visit(InvoiceAnalysisDocument document)
    {
        return document.NetAmount + document.TaxAmount;
    }

    public decimal Visit(PaymentAnalysisDocument document)
    {
        return document.Amount;
    }

    public decimal Visit(JournalAnalysisDocument document)
    {
        return document.Lines.Sum(line => line.Debit);
    }
}

public sealed class CountAnalysisVisitor
    : IAccountingDocumentVisitor
{
    public decimal Visit(InvoiceAnalysisDocument document)
    {
        return 1m;
    }

    public decimal Visit(PaymentAnalysisDocument document)
    {
        return 1m;
    }

    public decimal Visit(JournalAnalysisDocument document)
    {
        return 1m;
    }
}

