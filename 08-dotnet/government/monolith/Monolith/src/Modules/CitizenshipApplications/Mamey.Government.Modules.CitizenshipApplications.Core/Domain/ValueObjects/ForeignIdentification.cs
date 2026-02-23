namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

internal class ForeignIdentification
{
    public ForeignIdentification(string? number = null, string? issuingCountry = null, DateTime? expiryDate = null)
    {
        Number = string.IsNullOrEmpty(number) ? string.Empty : number;
        IssuingCountry = string.IsNullOrEmpty(issuingCountry) ? string.Empty : issuingCountry;
        ExpiryDate = expiryDate;
    }

    public string Number { get; } = string.Empty;
    public string IssuingCountry { get; } = string.Empty;
    public DateTime? ExpiryDate { get; }
}
