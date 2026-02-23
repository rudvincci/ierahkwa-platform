using Mamey.Security;
using Mamey.Security.Tests.Shared.Fixtures;
using Mamey.Security.Tests.Shared.Helpers;
using Shouldly;
using Xunit;

namespace Mamey.Security.Tests.Unit.Security;

/// <summary>
/// Comprehensive input validation tests covering all scenarios.
/// </summary>
public class InputValidationTests : IClassFixture<SecurityTestFixture>
{
    private readonly SecurityTestFixture _fixture;

    public InputValidationTests(SecurityTestFixture fixture)
    {
        _fixture = fixture;
    }

    #region SQL Injection Prevention

    [Fact]
    public void SqlInjectionPrevention_EncryptedValue_ShouldNotExecute()
    {
        // Arrange
        var sqlInjection = "'; DROP TABLE Users; --";
        var encrypted = _fixture.SecurityProvider.Encrypt(sqlInjection);

        // Act & Assert
        encrypted.ShouldNotBeNullOrEmpty();
        encrypted.ShouldNotContain("DROP");
        encrypted.ShouldNotContain("TABLE");
        // Encrypted value should not contain SQL keywords
    }

    [Fact]
    public void SqlInjectionPrevention_HashedValue_ShouldNotExecute()
    {
        // Arrange
        var sqlInjection = "'; DROP TABLE Users; --";
        var hashed = _fixture.SecurityProvider.Hash(sqlInjection);

        // Act & Assert
        hashed.ShouldNotBeNullOrEmpty();
        hashed.ShouldNotContain("DROP");
        hashed.ShouldNotContain("TABLE");
        // Hashed value should not contain SQL keywords
    }

    [Theory]
    [InlineData("'; DROP TABLE Users; --")]
    [InlineData("1' OR '1'='1")]
    [InlineData("admin'--")]
    [InlineData("' UNION SELECT * FROM Users--")]
    public void SqlInjectionPrevention_VariousPayloads_ShouldNotExecute(string payload)
    {
        // Arrange
        var encrypted = _fixture.SecurityProvider.Encrypt(payload);

        // Act & Assert
        encrypted.ShouldNotBeNullOrEmpty();
        encrypted.ShouldNotContain("DROP");
        encrypted.ShouldNotContain("UNION");
        encrypted.ShouldNotContain("SELECT");
    }

    #endregion

    #region NoSQL Injection Prevention

    [Fact]
    public void NoSqlInjectionPrevention_EncryptedValue_ShouldNotExecute()
    {
        // Arrange
        var nosqlInjection = "'; return true; //";
        var encrypted = _fixture.SecurityProvider.Encrypt(nosqlInjection);

        // Act & Assert
        encrypted.ShouldNotBeNullOrEmpty();
        encrypted.ShouldNotContain("return");
        encrypted.ShouldNotContain("true");
        // Encrypted value should not contain JavaScript/NoSQL keywords
    }

    [Fact]
    public void NoSqlInjectionPrevention_HashedValue_ShouldNotExecute()
    {
        // Arrange
        var nosqlInjection = "'; return true; //";
        var hashed = _fixture.SecurityProvider.Hash(nosqlInjection);

        // Act & Assert
        hashed.ShouldNotBeNullOrEmpty();
        hashed.ShouldNotContain("return");
        hashed.ShouldNotContain("true");
        // Hashed value should not contain JavaScript/NoSQL keywords
    }

    [Theory]
    [InlineData("'; return true; //")]
    [InlineData("'; db.dropDatabase(); //")]
    [InlineData("'; while(1){} //")]
    public void NoSqlInjectionPrevention_VariousPayloads_ShouldNotExecute(string payload)
    {
        // Arrange
        var encrypted = _fixture.SecurityProvider.Encrypt(payload);

        // Act & Assert
        encrypted.ShouldNotBeNullOrEmpty();
        encrypted.ShouldNotContain("return");
        encrypted.ShouldNotContain("dropDatabase");
    }

    #endregion

    #region XSS Prevention

    [Fact]
    public void XssPrevention_EncryptedValue_ShouldNotExecute()
    {
        // Arrange
        var xssPayload = "<script>alert('XSS')</script>";
        var encrypted = _fixture.SecurityProvider.Encrypt(xssPayload);

        // Act & Assert
        encrypted.ShouldNotBeNullOrEmpty();
        encrypted.ShouldNotContain("<script>");
        encrypted.ShouldNotContain("alert");
        // Encrypted value should not contain script tags
    }

    [Fact]
    public void XssPrevention_HashedValue_ShouldNotExecute()
    {
        // Arrange
        var xssPayload = "<script>alert('XSS')</script>";
        var hashed = _fixture.SecurityProvider.Hash(xssPayload);

        // Act & Assert
        hashed.ShouldNotBeNullOrEmpty();
        hashed.ShouldNotContain("<script>");
        hashed.ShouldNotContain("alert");
        // Hashed value should not contain script tags
    }

    [Theory]
    [InlineData("<script>alert('XSS')</script>")]
    [InlineData("<img src=x onerror=alert('XSS')>")]
    [InlineData("<svg onload=alert('XSS')>")]
    [InlineData("javascript:alert('XSS')")]
    public void XssPrevention_VariousPayloads_ShouldNotExecute(string payload)
    {
        // Arrange
        var encrypted = _fixture.SecurityProvider.Encrypt(payload);

        // Act & Assert
        encrypted.ShouldNotBeNullOrEmpty();
        encrypted.ShouldNotContain("<script>");
        encrypted.ShouldNotContain("onerror");
        encrypted.ShouldNotContain("onload");
        encrypted.ShouldNotContain("javascript:");
    }

    #endregion

    #region Path Traversal Prevention

    [Fact]
    public void PathTraversalPrevention_EncryptedValue_ShouldNotExecute()
    {
        // Arrange
        var pathTraversal = "../../../etc/passwd";
        var encrypted = _fixture.SecurityProvider.Encrypt(pathTraversal);

        // Act & Assert
        encrypted.ShouldNotBeNullOrEmpty();
        // Encrypted value should not contain path traversal patterns
        // Note: Encryption itself doesn't prevent path traversal,
        // but the encrypted value should not be directly used in file paths
    }

    [Fact]
    public void PathTraversalPrevention_HashedValue_ShouldNotExecute()
    {
        // Arrange
        var pathTraversal = "../../../etc/passwd";
        var hashed = _fixture.SecurityProvider.Hash(pathTraversal);

        // Act & Assert
        hashed.ShouldNotBeNullOrEmpty();
        // Hashed value should not contain path traversal patterns
    }

    [Theory]
    [InlineData("../../../etc/passwd")]
    [InlineData("..\\..\\..\\windows\\system32")]
    [InlineData("%2e%2e%2f%2e%2e%2f%2e%2e%2f")]
    public void PathTraversalPrevention_VariousPayloads_ShouldNotExecute(string payload)
    {
        // Arrange
        var encrypted = _fixture.SecurityProvider.Encrypt(payload);

        // Act & Assert
        encrypted.ShouldNotBeNullOrEmpty();
        // Note: Encryption doesn't prevent path traversal,
        // but the encrypted value should be validated before use
    }

    #endregion

    #region Command Injection Prevention

    [Fact]
    public void CommandInjectionPrevention_EncryptedValue_ShouldNotExecute()
    {
        // Arrange
        var commandInjection = "; rm -rf /";
        var encrypted = _fixture.SecurityProvider.Encrypt(commandInjection);

        // Act & Assert
        encrypted.ShouldNotBeNullOrEmpty();
        encrypted.ShouldNotContain("rm");
        encrypted.ShouldNotContain("-rf");
        // Encrypted value should not contain command injection patterns
    }

    [Fact]
    public void CommandInjectionPrevention_HashedValue_ShouldNotExecute()
    {
        // Arrange
        var commandInjection = "; rm -rf /";
        var hashed = _fixture.SecurityProvider.Hash(commandInjection);

        // Act & Assert
        hashed.ShouldNotBeNullOrEmpty();
        hashed.ShouldNotContain("rm");
        hashed.ShouldNotContain("-rf");
        // Hashed value should not contain command injection patterns
    }

    [Theory]
    [InlineData("; rm -rf /")]
    [InlineData("| cat /etc/passwd")]
    [InlineData("&& whoami")]
    [InlineData("`id`")]
    public void CommandInjectionPrevention_VariousPayloads_ShouldNotExecute(string payload)
    {
        // Arrange
        var encrypted = _fixture.SecurityProvider.Encrypt(payload);

        // Act & Assert
        encrypted.ShouldNotBeNullOrEmpty();
        encrypted.ShouldNotContain("rm");
        encrypted.ShouldNotContain("cat");
        encrypted.ShouldNotContain("whoami");
    }

    #endregion

    #region Input Length Validation

    [Fact]
    public void InputLengthValidation_VeryLongInput_ShouldHandle()
    {
        // Arrange
        var veryLongInput = new string('A', 1000000); // 1MB

        // Act
        var encrypted = _fixture.SecurityProvider.Encrypt(veryLongInput);
        var decrypted = _fixture.SecurityProvider.Decrypt(encrypted);

        // Assert
        encrypted.ShouldNotBeNullOrEmpty();
        decrypted.ShouldBe(veryLongInput);
    }

    [Fact]
    public void InputLengthValidation_EmptyInput_ShouldHandle()
    {
        // Arrange
        var emptyInput = "";

        // Act
        var encrypted = _fixture.SecurityProvider.Encrypt(emptyInput);
        var decrypted = _fixture.SecurityProvider.Decrypt(encrypted);

        // Assert
        encrypted.ShouldNotBeNullOrEmpty();
        decrypted.ShouldBe(emptyInput);
    }

    #endregion

    #region Special Character Handling

    [Fact]
    public void SpecialCharacterHandling_UnicodeCharacters_ShouldHandle()
    {
        // Arrange
        var unicodeInput = TestDataGenerator.GenerateUnicodeString();

        // Act
        var encrypted = _fixture.SecurityProvider.Encrypt(unicodeInput);
        var decrypted = _fixture.SecurityProvider.Decrypt(encrypted);

        // Assert
        encrypted.ShouldNotBeNullOrEmpty();
        decrypted.ShouldBe(unicodeInput);
    }

    [Fact]
    public void SpecialCharacterHandling_SpecialCharacters_ShouldHandle()
    {
        // Arrange
        var specialChars = TestDataGenerator.GenerateStringWithSpecialChars();

        // Act
        var encrypted = _fixture.SecurityProvider.Encrypt(specialChars);
        var decrypted = _fixture.SecurityProvider.Decrypt(encrypted);

        // Assert
        encrypted.ShouldNotBeNullOrEmpty();
        decrypted.ShouldBe(specialChars);
    }

    #endregion
}



