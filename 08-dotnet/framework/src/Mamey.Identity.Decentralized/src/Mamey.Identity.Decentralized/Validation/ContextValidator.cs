namespace Mamey.Identity.Decentralized.Validation;

/// <summary>
/// Validates the @context property for JSON-LD correctness and required vocabularies.
/// </summary>
public static class ContextValidator
{
    /// <summary>
    /// Checks that the required DID context is present.
    /// </summary>
    public static void Validate(IReadOnlyList<string> context)
    {
        if (context == null || !context.Contains(Core.DidContextConstants.W3cDidContext))
            throw new ArgumentException("DID Document must include the W3C DID context URI.");
    }
}