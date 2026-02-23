using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Domain.Entities;

public class MfaConfigurationTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateMfaConfiguration()
    {
        // Arrange
        var id = new MfaConfigurationId();
        var identityId = new IdentityId(Guid.NewGuid());
        var method = MfaMethod.Totp;

        // Act
        var config = new MfaConfiguration(id, identityId, method);

        // Assert
        config.ShouldNotBeNull();
        config.Id.ShouldBe(id);
        config.IdentityId.ShouldBe(identityId);
        config.Method.ShouldBe(method);
        config.IsEnabled.ShouldBeFalse();
        config.Events.ShouldContain(e => e is MfaConfigurationCreated);
    }

    [Fact]
    public void Enable_ShouldSetIsEnabledToTrue()
    {
        // Arrange
        var config = CreateTestMfaConfiguration();

        // Act
        config.Enable();

        // Assert
        config.IsEnabled.ShouldBeTrue();
        config.EnabledAt.ShouldNotBeNull();
    }

    [Fact]
    public void Disable_ShouldSetIsEnabledToFalse()
    {
        // Arrange
        var config = CreateTestMfaConfiguration();
        config.Enable();

        // Act
        config.Disable();

        // Assert
        config.IsEnabled.ShouldBeFalse();
    }

    [Fact]
    public void Enable_WithSecretKey_ShouldSetSecretKey()
    {
        // Arrange
        var config = CreateTestMfaConfiguration();
        var secretKey = "secret-key-123";

        // Act
        config.Enable(secretKey);

        // Assert
        config.SecretKey.ShouldBe(secretKey);
        config.IsEnabled.ShouldBeTrue();
    }

    [Fact]
    public void SetBackupCodes_WithValidCodes_ShouldSetBackupCodes()
    {
        // Arrange
        var config = CreateTestMfaConfiguration();
        var backupCodes = new List<string> { "code1", "code2", "code3" };

        // Act
        config.SetBackupCodes(backupCodes);

        // Assert
        config.BackupCodes.ShouldBe(backupCodes);
    }

    [Fact]
    public void UpdateLastUsed_ShouldUpdateLastUsedAt()
    {
        // Arrange
        var config = CreateTestMfaConfiguration();

        // Act
        config.UpdateLastUsed();

        // Assert
        config.LastUsedAt.ShouldNotBeNull();
    }

    private MfaConfiguration CreateTestMfaConfiguration()
    {
        return new MfaConfiguration(
            new MfaConfigurationId(),
            new IdentityId(Guid.NewGuid()),
            MfaMethod.Totp);
    }
}

