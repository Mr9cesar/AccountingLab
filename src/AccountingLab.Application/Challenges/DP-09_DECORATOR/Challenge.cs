using AccountingLab.Domain.Education;

namespace AccountingLab.Application.Challenges.DP09Decorator;

public sealed record DocumentExportRequest(string Body, IReadOnlyList<string> Features, IReadOnlyDictionary<string, string> Metadata);
public sealed record ExportedAccountingDocument(string Content, IReadOnlyList<string> AppliedFeatures, IReadOnlyDictionary<string, string> Metadata);
public interface IDocumentEnrichmentChallenge { ExportedAccountingDocument Export(DocumentExportRequest request); }
public sealed class DocumentEnrichmentChallenge : IDocumentEnrichmentChallenge
{
    public ExportedAccountingDocument Export(DocumentExportRequest request)
    {
        IAccountingDocument document = new BasicAccountingDocument(request.Body);

        foreach (var feature in request.Features)
        {
            document = feature switch
            {
                "auditoria" => new AuditDocumentDecorator(document),
                "metadatos" => new MetadataDocumentDecorator(document, request.Metadata),
                _ => throw new ArgumentException("Característica no válida.")
            };
        }

        return document.Export();
    }
}

public interface IAccountingDocument
{
    ExportedAccountingDocument Export();
}

public sealed class BasicAccountingDocument : IAccountingDocument
{
    private readonly string _body;

    public BasicAccountingDocument(string body)
    {
        _body = body;
    }

    public ExportedAccountingDocument Export()
    {
        return new ExportedAccountingDocument(
            _body,
            Array.Empty<string>(),
            new Dictionary<string, string>());
    }
}

public sealed class AuditDocumentDecorator : IAccountingDocument
{
    private readonly IAccountingDocument _document;

    public AuditDocumentDecorator(IAccountingDocument document)
    {
        _document = document;
    }

    public ExportedAccountingDocument Export()
    {
        var result = _document.Export();

        return result with
        {
            Content = result.Content + "\n[AUDIT]",
            AppliedFeatures = result.AppliedFeatures
                .Append("auditoria").ToArray()
        };
    }
}

public sealed class MetadataDocumentDecorator : IAccountingDocument
{
    private readonly IAccountingDocument _document;
    private readonly IReadOnlyDictionary<string, string> _metadata;

    public MetadataDocumentDecorator(
        IAccountingDocument document,
        IReadOnlyDictionary<string, string> metadata)
    {
        _document = document;
        _metadata = metadata;
    }

    public ExportedAccountingDocument Export()
    {
        var result = _document.Export();

        return result with
        {
            Metadata = new Dictionary<string, string>(_metadata),
            AppliedFeatures = result.AppliedFeatures
                .Append("metadatos").ToArray()
        };
    }
}

