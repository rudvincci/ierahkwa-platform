namespace Mamey.Government.UI.Models;

/// <summary>
/// Request to reject an application.
/// </summary>
public class RejectApplicationRequest
{
    public string Reason { get; set; } = string.Empty;
}