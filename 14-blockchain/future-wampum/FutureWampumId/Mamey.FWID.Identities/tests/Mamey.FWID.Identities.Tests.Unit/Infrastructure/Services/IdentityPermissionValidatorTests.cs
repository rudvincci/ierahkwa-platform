using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mamey.FWID.Identities.Infrastructure.Security;
using Mamey.WebApi.Security;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Mamey.FWID.Identities.Tests.Unit.Infrastructure.Services;

public class IdentityPermissionValidatorTests
{
    private readonly ILogger<IdentityPermissionValidator> _logger;
    private readonly IdentityPermissionValidator _validator;
    private readonly X509Certificate2 _certificate;

    public IdentityPermissionValidatorTests()
    {
        _logger = Substitute.For<ILogger<IdentityPermissionValidator>>();
        _validator = new IdentityPermissionValidator(_logger);
        
        // Create a test certificate (in real tests, use a proper test certificate)
        var rsa = System.Security.Cryptography.RSA.Create();
        var certRequest = new System.Security.Cryptography.X509Certificates.CertificateRequest(
            "CN=test-cert", rsa, System.Security.Cryptography.HashAlgorithmName.SHA256, System.Security.Cryptography.RSASignaturePadding.Pkcs1);
        _certificate = certRequest.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(1));
    }

    [Fact]
    public void HasAccess_WithAdminPermission_ShouldGrantAllPermissions()
    {
        // Arrange
        var grantedPermissions = new[] { "identities:admin" };
        var requiredPermissions = new[] { "identities:read", "identities:write", "identities:verify" };
        var context = CreateHttpContext("GET", "/api/identities");

        // Act
        var result = _validator.HasAccess(_certificate, grantedPermissions, context);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasAccess_WithWritePermission_ShouldGrantReadAndVerify()
    {
        // Arrange
        var grantedPermissions = new[] { "identities:write" };
        var requiredPermissions = new[] { "identities:read", "identities:verify" };
        var context = CreateHttpContext("POST", "/api/identities");

        // Act
        var result = _validator.HasAccess(_certificate, grantedPermissions, context);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasAccess_WithVerifyPermission_ShouldGrantRead()
    {
        // Arrange
        var grantedPermissions = new[] { "identities:verify" };
        var context = CreateHttpContext("POST", "/api/identities/{id}/verify");

        // Act
        var result = _validator.HasAccess(_certificate, grantedPermissions, context);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasAccess_WithReadPermission_ShouldGrantReadOnly()
    {
        // Arrange
        var grantedPermissions = new[] { "identities:read" };
        var context = CreateHttpContext("GET", "/api/identities");

        // Act
        var result = _validator.HasAccess(_certificate, grantedPermissions, context);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasAccess_WithInsufficientPermissions_ShouldDenyAccess()
    {
        // Arrange
        var grantedPermissions = new[] { "identities:read" };
        var context = CreateHttpContext("POST", "/api/identities");

        // Act
        var result = _validator.HasAccess(_certificate, grantedPermissions, context);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void HasAccess_WithNullCertificate_ShouldDenyAccess()
    {
        // Arrange
        var grantedPermissions = new[] { "identities:read" };
        var context = CreateHttpContext("GET", "/api/identities");

        // Act
        var result = _validator.HasAccess(null!, grantedPermissions, context);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void HasAccess_WithNoGrantedPermissions_ShouldDenyAccess()
    {
        // Arrange
        var grantedPermissions = Array.Empty<string>();
        var context = CreateHttpContext("GET", "/api/identities");

        // Act
        var result = _validator.HasAccess(_certificate, grantedPermissions, context);

        // Assert
        result.ShouldBeFalse();
    }

    [Fact]
    public void HasAccess_WithGrpcMethod_ShouldCheckGrpcPermissions()
    {
        // Arrange
        var grantedPermissions = new[] { "identities:verify" };
        var context = CreateHttpContext("POST", "/Services.Biometric.BiometricService/VerifyBiometric");
        context.Request.ContentType = "application/grpc";

        // Act
        var result = _validator.HasAccess(_certificate, grantedPermissions, context);

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void HasAccess_WithRequiredPermissionsInContext_ShouldUseContextPermissions()
    {
        // Arrange
        var grantedPermissions = new[] { "identities:admin" };
        var requiredPermissions = new HashSet<string> { "identities:read" };
        var context = CreateHttpContext("GET", "/api/identities");
        context.Items["RequiredPermissions"] = requiredPermissions;

        // Act
        var result = _validator.HasAccess(_certificate, grantedPermissions, context);

        // Assert
        result.ShouldBeTrue();
    }

    private HttpContext CreateHttpContext(string method, string path)
    {
        var context = Substitute.For<HttpContext>();
        var request = Substitute.For<HttpRequest>();
        var response = Substitute.For<HttpResponse>();
        var items = new Dictionary<object, object?>();

        request.Method.Returns(method);
        request.Path.Returns(new PathString(path));
        request.ContentType.Returns((string?)null);
        context.Request.Returns(request);
        context.Response.Returns(response);
        context.Items.Returns(items);

        return context;
    }
}

