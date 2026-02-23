using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using Mamey.WebApi.Security;

namespace Mamey.FWID.Notifications.Infrastructure.Grpc.Interceptors;

/// <summary>
/// gRPC interceptor for certificate-based authentication (internal service-to-service communication)
/// </summary>
public class CertificateAuthenticationInterceptor : Interceptor
{
    private readonly ILogger<CertificateAuthenticationInterceptor> _logger;
    private readonly SecurityOptions _securityOptions;
    private readonly ICertificatePermissionValidator _certificatePermissionValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CertificateAuthenticationInterceptor(
        ILogger<CertificateAuthenticationInterceptor> logger,
        SecurityOptions securityOptions,
        ICertificatePermissionValidator certificatePermissionValidator,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _securityOptions = securityOptions;
        _certificatePermissionValidator = certificatePermissionValidator;
        _httpContextAccessor = httpContextAccessor;
    }

    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            // Check if certificate authentication is enabled
            if (_securityOptions.Certificate == null || !_securityOptions.Certificate.Enabled)
            {
                return continuation(request, context);
            }

            // Extract certificate from context
            var certificate = ExtractCertificate(context);
            
            if (certificate == null)
            {
                // If no certificate, try JWT authentication (external clients)
                // This allows both authentication methods
                _logger.LogDebug("No certificate found for gRPC call: {Method}, allowing JWT fallback", context.Method);
                return continuation(request, context);
            }

            // Verify certificate
            if (!VerifyCertificate(certificate))
            {
                _logger.LogWarning("Invalid certificate for gRPC call: {Method}, Subject: {Subject}", 
                    context.Method, certificate.Subject);
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid certificate"));
            }

            // Validate ACL if configured
            if (_securityOptions.Certificate.Acl != null && _securityOptions.Certificate.Acl.Any())
            {
                var subject = certificate.Subject;
                if (!_securityOptions.Certificate.Acl.TryGetValue(subject, out var acl))
                {
                    _logger.LogWarning("Certificate subject not in ACL: {Subject}", subject);
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Certificate not authorized"));
                }

                // Validate issuer if specified
                if (!string.IsNullOrWhiteSpace(acl.ValidIssuer) && certificate.Issuer != acl.ValidIssuer)
                {
                    _logger.LogWarning("Certificate issuer mismatch: {Issuer}, Expected: {ExpectedIssuer}", 
                        certificate.Issuer, acl.ValidIssuer);
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Certificate issuer not authorized"));
                }

                // Validate permissions if specified
                if (acl.Permissions != null && acl.Permissions.Any())
                {
                    var hasPermission = _certificatePermissionValidator.Validate(certificate, acl.Permissions);
                    if (!hasPermission)
                    {
                        _logger.LogWarning("Certificate does not have required permissions: {Subject}", subject);
                        throw new RpcException(new Status(StatusCode.PermissionDenied, "Insufficient permissions"));
                    }
                }
            }

            // Add certificate info to gRPC context
            context.UserState["Certificate"] = certificate;
            context.UserState["CertificateSubject"] = certificate.Subject;
            
            _logger.LogDebug("Certificate authentication successful for gRPC call: {Method}, Subject: {Subject}", 
                context.Method, certificate.Subject);

            return continuation(request, context);
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in certificate authentication interceptor for gRPC call: {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.Internal, "Authentication error"));
        }
    }

    private X509Certificate2? ExtractCertificate(ServerCallContext context)
    {
        // Try to get certificate from HTTP context (if available)
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var clientCertificate = httpContext.Connection.ClientCertificate;
            if (clientCertificate != null)
            {
                return clientCertificate;
            }

            // Try to get certificate from header
            var certificateHeader = _securityOptions.Certificate?.Header ?? "Certificate";
            if (httpContext.Request.Headers.TryGetValue(certificateHeader, out var headerValue))
            {
                try
                {
                    var certificateData = Convert.FromBase64String(headerValue.ToString());
                    return new X509Certificate2(certificateData);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse certificate from header: {Header}", certificateHeader);
                }
            }
        }

        return null;
    }

    private bool VerifyCertificate(X509Certificate2 certificate)
    {
        try
        {
            // Basic certificate validation
            if (!certificate.Verify())
            {
                return false;
            }

            // Check expiration
            if (DateTime.UtcNow > certificate.NotAfter || DateTime.UtcNow < certificate.NotBefore)
            {
                _logger.LogWarning("Certificate expired or not yet valid: {Subject}, NotBefore: {NotBefore}, NotAfter: {NotAfter}",
                    certificate.Subject, certificate.NotBefore, certificate.NotAfter);
                return false;
            }

            // Additional validation can be added here (e.g., revocation check)
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying certificate: {Subject}", certificate.Subject);
            return false;
        }
    }
}







