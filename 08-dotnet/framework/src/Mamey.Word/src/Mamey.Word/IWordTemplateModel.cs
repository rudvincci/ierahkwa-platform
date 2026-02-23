namespace Mamey.Word;

public interface IWordTemplateModel : IWordTemplateModel<Guid>
{
    
}
public interface IWordTemplateModel<T>
{
    string? Prefix { get; }
    T SerialNumber { get; }
    string Resource { get; }
    string DocumentTitle { get; }
    string DocumentVerificationUrl { get; }
    WordDocumentProperties DocumentProperties { get; }
    Dictionary<string, byte[]>? ImageSelectors { get; set; }
}