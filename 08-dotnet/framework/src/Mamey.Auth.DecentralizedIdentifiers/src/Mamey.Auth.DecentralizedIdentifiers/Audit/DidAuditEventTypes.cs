namespace Mamey.Auth.DecentralizedIdentifiers.Audit;

/// <summary>
/// Defines audit event types specific to DID operations.
/// </summary>
public static class DidAuditEventTypes
{
    /// <summary>
    /// DID creation events.
    /// </summary>
    public const string DID_CREATION = "DID_CREATION";
    
    /// <summary>
    /// DID resolution events.
    /// </summary>
    public const string DID_RESOLUTION = "DID_RESOLUTION";
    
    /// <summary>
    /// DID update events.
    /// </summary>
    public const string DID_UPDATE = "DID_UPDATE";
    
    /// <summary>
    /// DID deactivation events.
    /// </summary>
    public const string DID_DEACTIVATION = "DID_DEACTIVATION";
    
    /// <summary>
    /// Verifiable Credential issuance events.
    /// </summary>
    public const string VC_ISSUANCE = "VC_ISSUANCE";
    
    /// <summary>
    /// Verifiable Credential verification events.
    /// </summary>
    public const string VC_VERIFICATION = "VC_VERIFICATION";
    
    /// <summary>
    /// Verifiable Credential revocation events.
    /// </summary>
    public const string VC_REVOCATION = "VC_REVOCATION";
    
    /// <summary>
    /// Verifiable Presentation creation events.
    /// </summary>
    public const string VP_CREATION = "VP_CREATION";
    
    /// <summary>
    /// Verifiable Presentation verification events.
    /// </summary>
    public const string VP_VERIFICATION = "VP_VERIFICATION";
    
    /// <summary>
    /// DID-based authentication events.
    /// </summary>
    public const string DID_AUTHENTICATION = "DID_AUTHENTICATION";
    
    /// <summary>
    /// DID method-specific operations.
    /// </summary>
    public const string DID_METHOD_OPERATION = "DID_METHOD_OPERATION";
    
    /// <summary>
    /// Key management operations.
    /// </summary>
    public const string KEY_MANAGEMENT = "KEY_MANAGEMENT";
    
    /// <summary>
    /// Proof creation and verification events.
    /// </summary>
    public const string PROOF_OPERATION = "PROOF_OPERATION";
}

/// <summary>
/// Defines audit categories for DID operations.
/// </summary>
public static class DidAuditCategories
{
    /// <summary>
    /// General DID operations category.
    /// </summary>
    public const string DID_OPERATIONS = "DID_OPERATIONS";
    
    /// <summary>
    /// Verifiable Credential operations category.
    /// </summary>
    public const string CREDENTIAL_OPERATIONS = "CREDENTIAL_OPERATIONS";
    
    /// <summary>
    /// Authentication operations category.
    /// </summary>
    public const string AUTHENTICATION = "AUTHENTICATION";
    
    /// <summary>
    /// Security-related operations category.
    /// </summary>
    public const string SECURITY = "SECURITY";
    
    /// <summary>
    /// Key management operations category.
    /// </summary>
    public const string KEY_MANAGEMENT = "KEY_MANAGEMENT";
    
    /// <summary>
    /// Proof operations category.
    /// </summary>
    public const string PROOF_OPERATIONS = "PROOF_OPERATIONS";
}

/// <summary>
/// Defines audit status values for DID operations.
/// </summary>
public static class DidAuditStatus
{
    /// <summary>
    /// Operation completed successfully.
    /// </summary>
    public const string SUCCESS = "SUCCESS";
    
    /// <summary>
    /// Operation failed.
    /// </summary>
    public const string FAILURE = "FAILURE";
    
    /// <summary>
    /// Operation is in progress.
    /// </summary>
    public const string IN_PROGRESS = "IN_PROGRESS";
    
    /// <summary>
    /// Operation was cancelled.
    /// </summary>
    public const string CANCELLED = "CANCELLED";
    
    /// <summary>
    /// Operation timed out.
    /// </summary>
    public const string TIMEOUT = "TIMEOUT";
}







