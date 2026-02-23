namespace Mamey.Government.Modules.CitizenshipApplications.Contracts.DTO;

public class ResumeApplicationResponse
{
    public string ApplicationNumber { get; set; } = string.Empty;
    public string JwtToken { get; set; } = string.Empty;
    public ApplicationDto? Application { get; set; } // Optional, for future use
}
