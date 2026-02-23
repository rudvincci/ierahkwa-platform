using System;
using FluentAssertions;
using Mamey.Types;
using Pupitre.Users.Domain.Entities;
using Xunit;

namespace Pupitre.Users.Tests.Unit.Domain;

public class UserTests
{
    [Fact]
    public void Create_WithValidData_ShouldInitializeAggregate()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Jordan Rivers";
        var tags = new[] { "beta", "student" };
        var citizenId = "ST-1001";

        // Act
        var user = User.Create(
            id: id,
            name: name,
            tags: tags,
            citizenId: citizenId,
            nationality: "USA",
            programCode: "STEM-ADV",
            credentialType: "Profile");

        // Assert
        user.Should().NotBeNull();
        user.Id.Value.Should().Be(id);
        user.Name.Should().Be(name);
        user.Tags.Should().BeEquivalentTo(tags);
        user.CitizenId.Should().Be(citizenId);
        user.Nationality.Should().Be("USA");
        user.ProgramCode.Should().Be("STEM-ADV");
        user.CredentialType.Should().Be("Profile");
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        user.Version.Should().Be(0);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrow()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var act = () => User.Create(id, string.Empty, Array.Empty<string>());

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_ShouldMutateMutableFields()
    {
        // Arrange
        var user = User.Create(
            Guid.NewGuid(),
            "Initial Name",
            new[] { "math" },
            citizenId: "ST-2001",
            nationality: "USA",
            programCode: "STEM-CORE",
            credentialType: "Profile");

        var newTags = new[] { "math", "robotics" };
        var newProgram = "ROBO-LAB";

        // Act
        user.Update(
            name: "Updated Name",
            tags: newTags,
            programCode: newProgram,
            credentialType: "Profile",
            completionDate: DateTime.UtcNow,
            metadata: new Dictionary<string, string> { ["advisor"] = "Coach Carter" },
            nationality: "USA");

        // Assert
        user.Name.Should().Be("Updated Name");
        user.Tags.Should().BeEquivalentTo(newTags);
        user.ProgramCode.Should().Be(newProgram);
        user.CredentialType.Should().Be("Profile");
        user.CredentialIssuedAt.Should().NotBeNull();
        user.GetBlockchainMetadata().Should().ContainKey("advisor");
    }
}
