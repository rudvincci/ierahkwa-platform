using Mamey.Types;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

public class PersonalInformation
{
    public Name Name { get; init; }
    public DateTime DateOfBirth { get; init; }
    public string PlaceOfBirth { get; init; } = string.Empty;
    public string CountryOfOrigin { get; init; } = string.Empty;
    public string Sex { get; set; } = string.Empty;
    public int HeightInInches { get; init; }
    public string EyeColor { get; init; } = string.Empty;
    public string HairColor { get; init; } = string.Empty;
    public MaritalStatus MaritalStatus { get; init; }
    public IEnumerable<string> PreviousNames { get; init; } = new List<string>();
}