namespace Mamey.Portal.Shared.Storage.DocumentNaming;

public interface IDocumentNamingService
{
    string GenerateObjectKey(DocumentNamingPattern pattern, DocumentNamingContext ctx);
}




