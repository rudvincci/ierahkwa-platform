using System.Net;

public interface IRequestMethod
{
    public IEnumerable<string> Tags { get; set; }
    public string Summary { get; set; }
    public string OperationId { get; set; }
    public IEnumerable<object> Parameters { get; set; } // TODO: Define 'object's' type
    public IRequestBody RequestBody { get; set; }
    public Dictionary<HttpStatusCode, IRequetResponses> Responses{ get; set; }
}
