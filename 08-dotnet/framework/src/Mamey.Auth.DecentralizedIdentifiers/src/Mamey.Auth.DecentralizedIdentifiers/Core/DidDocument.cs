using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Newtonsoft.Json.Linq;

namespace Mamey.Auth.DecentralizedIdentifiers.Core;

/// <summary>
/// Implements the W3C-compliant DID Document model.
/// </summary>
public class DidDocument : IDidDocument
{
    public IReadOnlyList<string> Context { get; }
    public string Id { get; }
    public IReadOnlyList<string> Controller { get; }
    public IReadOnlyList<IDidVerificationMethod> VerificationMethods { get; }
    public IReadOnlyList<object> Authentication { get; }
    public IReadOnlyList<object> AssertionMethod { get; }
    public IReadOnlyList<object> KeyAgreement { get; }
    public IReadOnlyList<object> CapabilityDelegation { get; }
    public IReadOnlyList<object> CapabilityInvocation { get; }
    public IReadOnlyList<IDidServiceEndpoint> ServiceEndpoints { get; }
    public IReadOnlyDictionary<string, object> AdditionalProperties { get; }

    /// <summary>
    /// Creates a new DID Document with required and optional values.
    /// </summary>
    public DidDocument(
        IEnumerable<string> context,
        string id,
        IEnumerable<string> controller = null,
        IEnumerable<IDidVerificationMethod> verificationMethods = null,
        IEnumerable<object> authentication = null,
        IEnumerable<object> assertionMethod = null,
        IEnumerable<object> keyAgreement = null,
        IEnumerable<object> capabilityDelegation = null,
        IEnumerable<object> capabilityInvocation = null,
        IEnumerable<IDidServiceEndpoint> serviceEndpoints = null,
        IDictionary<string, object> additionalProperties = null)
    {
        Context = context?.ToArray() ?? throw new ArgumentNullException(nameof(context));
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Controller = controller?.ToArray() ?? Array.Empty<string>();
        VerificationMethods = verificationMethods?.ToArray() ?? Array.Empty<IDidVerificationMethod>();
        Authentication = authentication?.ToArray() ?? Array.Empty<object>();
        AssertionMethod = assertionMethod?.ToArray() ?? Array.Empty<object>();
        KeyAgreement = keyAgreement?.ToArray() ?? Array.Empty<object>();
        CapabilityDelegation = capabilityDelegation?.ToArray() ?? Array.Empty<object>();
        CapabilityInvocation = capabilityInvocation?.ToArray() ?? Array.Empty<object>();
        ServiceEndpoints = serviceEndpoints?.ToArray() ?? Array.Empty<IDidServiceEndpoint>();
        AdditionalProperties = additionalProperties != null
            ? new Dictionary<string, object>(additionalProperties)
            : new Dictionary<string, object>();
    }

    /// <summary>
    /// Parses a JSON string into a strongly typed DidDocument.
    /// </summary>
    /// <param name="json">The JSON string representing a DID Document.</param>
    /// <returns>The parsed DidDocument.</returns>
    public static DidDocument Parse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON cannot be empty.", nameof(json));

        JObject jobj;
        try
        {
            jobj = JObject.Parse(json);
        }
        catch (Exception ex)
        {
            throw new FormatException("Invalid JSON format for DID Document.", ex);
        }

        // 1. @context (required, can be string or array)
        var contextToken = jobj["@context"];
        var context = contextToken switch
        {
            JArray arr => arr.Select(t => t.Type == JTokenType.String ? t.Value<string>() : t.ToString()).ToList(),
            JValue val when val.Type == JTokenType.String => new List<string> { val.Value<string>() },
            _ => throw new FormatException("@context must be a string or array of strings.")
        };

        // 2. id (required, must be non-empty string)
        var idToken = jobj["id"];
        if (idToken == null || idToken.Type == JTokenType.Null)
            throw new FormatException("Missing required 'id'.");
        if (idToken.Type != JTokenType.String)
            throw new FormatException("'id' must be a string.");
        var id = idToken.Value<string>();
        if (string.IsNullOrWhiteSpace(id))
            throw new FormatException("'id' must not be empty.");


        // 3. controller (optional, string or array)
        var controllerToken = jobj["controller"];
        var controller = controllerToken switch
        {
            JArray arr => arr.Select(x => x.Value<string>()).ToList(),
            JValue val when val.Type == JTokenType.String => new List<string> { val.Value<string>() },
            null => null,
            _ => throw new FormatException("controller must be string or array of strings.")
        };

        // 4. verificationMethod (optional)
        var verificationMethods = ParseVerificationMethods(jobj["verificationMethod"]);
        if (verificationMethods != null)
        {
            var duplicate = verificationMethods
                .GroupBy(vm => vm.Id)
                .Where(g => !string.IsNullOrWhiteSpace(g.Key) && g.Count() > 1)
                .Select(g => g.Key)
                .FirstOrDefault();
            if (duplicate != null)
                throw new FormatException($"Duplicate verificationMethod id detected: '{duplicate}'");
        }
        
        // 5. authentication (optional, array or value)
        var authentication = ParseObjectArray(jobj["authentication"]);

        // 6. assertionMethod (optional)
        var assertionMethod = ParseObjectArray(jobj["assertionMethod"]);

        // 7. keyAgreement (optional)
        var keyAgreement = ParseObjectArray(jobj["keyAgreement"]);

        // 8. capabilityDelegation (optional)
        var capabilityDelegation = ParseObjectArray(jobj["capabilityDelegation"]);

        // 9. capabilityInvocation (optional)
        var capabilityInvocation = ParseObjectArray(jobj["capabilityInvocation"]);

        // 10. service (optional)
        var serviceEndpoints = ParseServiceEndpoints(jobj["service"]);

        // 11. Additional properties (for extensibility)
        var knownProps = new[]
        {
            "@context", "id", "controller", "verificationMethod", "authentication", "assertionMethod",
            "keyAgreement", "capabilityDelegation", "capabilityInvocation", "service"
        };
        var additional = jobj.Properties()
            .Where(p => !knownProps.Contains(p.Name))
            .ToDictionary(p => p.Name, p => (object)p.Value);

        return new DidDocument(
            context,
            id,
            controller,
            verificationMethods,
            authentication,
            assertionMethod,
            keyAgreement,
            capabilityDelegation,
            capabilityInvocation,
            serviceEndpoints,
            additional
        );
    }

    public JObject ToJsonObject()
    {
        var obj = new JObject();

        // @context: array or string
        if (Context != null)
        {
            obj["@context"] = Context.Count == 1
                ? (JToken)Context[0]
                : new JArray(Context);
        }

        obj["id"] = Id;

        if (Controller != null && Controller.Any())
            obj["controller"] = Controller.Count == 1 ? (JToken)Controller[0] : new JArray(Controller);

        if (VerificationMethods != null && VerificationMethods.Any())
            obj["verificationMethod"] = new JArray(VerificationMethods.Select(vm =>
                (vm as VerificationMethod)?.ToJsonObject() ??
                throw new InvalidOperationException("VerificationMethod must support ToJsonObject()")));

        if (Authentication != null && Authentication.Any())
            obj["authentication"] = new JArray(Authentication);

        if (AssertionMethod != null && AssertionMethod.Any())
            obj["assertionMethod"] = new JArray(AssertionMethod);

        if (KeyAgreement != null && KeyAgreement.Any())
            obj["keyAgreement"] = new JArray(KeyAgreement);

        if (CapabilityDelegation != null && CapabilityDelegation.Any())
            obj["capabilityDelegation"] = new JArray(CapabilityDelegation);

        if (CapabilityInvocation != null && CapabilityInvocation.Any())
            obj["capabilityInvocation"] = new JArray(CapabilityInvocation);

        if (ServiceEndpoints != null && ServiceEndpoints.Any())
            obj["service"] = new JArray(ServiceEndpoints.Select(se =>
                (se as ServiceEndpoint)?.ToJsonObject() ??
                throw new InvalidOperationException("ServiceEndpoint must support ToJsonObject()")));

        // Additional/extensible properties
        if (AdditionalProperties != null)
        {
            foreach (var prop in AdditionalProperties)
            {
                if (!obj.ContainsKey(prop.Key))
                    obj[prop.Key] = prop.Value is JToken jt ? jt : JToken.FromObject(prop.Value);
            }
        }

        return obj;
    }

    public string ToJson()
    {
        return ToJsonObject().ToString();
    }

    /// <summary>
    /// Helper: Parse an array of verification methods from JToken.
    /// </summary>
    private static IReadOnlyList<IDidVerificationMethod> ParseVerificationMethods(JToken token)
    {
        if (token == null)
            return null;
        var arr = token.Type == JTokenType.Array ? (JArray)token : new JArray(token);
        var result = new List<IDidVerificationMethod>();
        foreach (var item in arr)
        {
            if (item.Type != JTokenType.Object)
                throw new FormatException("Each verificationMethod must be an object.");
            // Assume a DidVerificationMethod.Parse(JObject) exists:
            result.Add(VerificationMethod.Parse((JObject)item));
        }

        return result;
    }

    // Helper: Parse service endpoints array
    private static IReadOnlyList<IDidServiceEndpoint> ParseServiceEndpoints(JToken token)
    {
        if (token == null)
            return null;
        var arr = token.Type == JTokenType.Array ? (JArray)token : new JArray(token);
        var result = new List<IDidServiceEndpoint>();
        foreach (var item in arr)
        {
            if (item.Type != JTokenType.Object)
                throw new FormatException("Each service endpoint must be an object.");
            // Assume a DidServiceEndpoint.Parse(JObject) exists:
            result.Add(ServiceEndpoint.Parse((JObject)item));
        }

        return result;
    }

    // Helper: Accepts value or array, returns array of objects/strings
    private static IReadOnlyList<object> ParseObjectArray(JToken token)
    {
        if (token == null) return null;
        if (token.Type == JTokenType.Array)
            return token.Select(ParseValueOrObject).ToList();
        return new List<object> { ParseValueOrObject(token) };
    }

    // Accepts string, object, int, etc.
    private static object ParseValueOrObject(JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.String:
                return token.Value<string>();
            case JTokenType.Object:
                return token.ToObject<Dictionary<string, object>>();
            case JTokenType.Integer:
            case JTokenType.Float:
                return token.Value<double>();
            case JTokenType.Boolean:
                return token.Value<bool>();
            default:
                return token.ToString();
        }
    }
}