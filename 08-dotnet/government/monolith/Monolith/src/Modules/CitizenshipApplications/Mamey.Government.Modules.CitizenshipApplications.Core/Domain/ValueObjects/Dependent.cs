using System.Text.Json.Serialization;
using Mamey.Types;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;

internal class Dependent
{
    [JsonConstructor]
    public Dependent(Name name, DateTime dateOfBirth, Gender gender)
    {
        Name = name;
        DateOfBirth = dateOfBirth;
        Gender = gender;
    }

    public Name Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public int Age => DateOfBirth.CalculateAge();
}
