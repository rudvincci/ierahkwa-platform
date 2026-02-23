namespace Mamey.Government.UI.Models;

/// <summary>
/// Request to start a citizenship application.
/// </summary>
public class StartApplicationRequest
{
    public string Email { get; set; } = string.Empty;
}