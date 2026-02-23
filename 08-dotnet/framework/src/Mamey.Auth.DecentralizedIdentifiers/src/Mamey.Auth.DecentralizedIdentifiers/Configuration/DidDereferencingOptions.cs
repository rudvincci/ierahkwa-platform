namespace Mamey.Auth.DecentralizedIdentifiers.Configuration;

/// <summary>
/// Options for dereferencing DID URLs, fragments, and related services.
/// </summary>
public class DidDereferencingOptions
{
    public bool AllowFragmentDereferencing { get; set; } = true;
    public bool EnableRemoteDereferencing { get; set; } = false;
    public int DereferencingMaxDepth { get; set; } = 3;
}