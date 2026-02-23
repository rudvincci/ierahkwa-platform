using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

internal class PersonalDetails
{
    public PersonalDetails()
    {
    }

    [JsonConstructor]
    public PersonalDetails(DateTime dateOfBirth, string placeOfBirth,
        EyeColor eyeColor, HairColor hairColor, double heightInInches,
        double weightInPounds, Gender gender, MaritalStatus maritalStatus = MaritalStatus.Single,
        Name? spouseName = null, List<string>? currentNationalities = null,
        List<string>? aliases = null)
    {
        Aliases = aliases ?? new List<string>();
        DateOfBirth = dateOfBirth;
        PlaceOfBirth = placeOfBirth;
        EyeColor = eyeColor;
        HairColor = hairColor;
        HeightInInches = heightInInches;
        WeightInPounds = weightInPounds;
        Gender = gender;
        MaritalStatus = maritalStatus;
        SpouseName = spouseName;
        CurrentNationalities = currentNationalities ?? new List<string>();
    }

    public DateTime DateOfBirth { get; set; }
    public string PlaceOfBirth { get; set; } = string.Empty;
    public EyeColor EyeColor { get; set; }
    public HairColor HairColor { get; set; }
    public double HeightInInches { get; set; }
    public double WeightInPounds { get; set; }
    public Gender Gender { get; set; }
    public MaritalStatus MaritalStatus { get; set; } = MaritalStatus.Single;
    public List<string> Aliases { get; set; } = new List<string>();
    public Name? SpouseName { get; set; } = null;
    public List<string> CurrentNationalities { get; set; } = new List<string>();
}
