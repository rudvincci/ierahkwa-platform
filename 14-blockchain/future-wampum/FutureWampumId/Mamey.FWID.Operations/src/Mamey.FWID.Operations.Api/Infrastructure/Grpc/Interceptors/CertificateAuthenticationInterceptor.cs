using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using Mamey.WebApi.Security;

namespace Mamey.FWID.Operations.Api.Infrastructure.Grpc.Interceptors;

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

                // Validate thumbprint if specified
                if (!string.IsNullOrWhiteSpace(acl.ValidThumbprint) && certificate.Thumbprint != acl.ValidThumbprint)
                {
                    _logger.LogWarning("Certificate thumbprint mismatch: {Thumbprint}, Expected: {ExpectedThumbprint}", 
                        certificate.Thumbprint, acl.ValidThumbprint);
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Certificate thumbprint not authorized"));
                }

                // Validate permissions if specified
                if (acl.Permissions != null && acl.Permissions.Any())
                {
                    var httpContext = _httpContextAccessor.HttpContext;
                    if (httpContext != null && !_certificatePermissionValidator.HasAccess(certificate, acl.Permissions, httpContext))
                    {
                        _logger.LogWarning("Certificate does not have required permissions: {Subject}", subject);
                        throw new RpcException(new Status(StatusCode.PermissionDenied, "Insufficient permissions"));
                    }
                }
            }

            // Add certificate info to context
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
            _logger.LogError(ex, "Error during certificate authentication for gRPC call: {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.Internal, "Authentication error"));
        }
    }

    private X509Certificate2? ExtractCertificate(ServerCallContext context)
    {
        // Try to extract certificate from HTTP context (if available)
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            // Check connection certificate
            var connectionCert = httpContext.Connection.ClientCertificate;
            if (connectionCert != null)
            {
                return connectionCert;
            }

            // Check forwarded certificate header
            var certHeader = _securityOptions.Certificate.GetHeaderName();
            if (httpContext.Request.Headers.TryGetValue(certHeader, out var headerValue))
            {
                try
                {
                    var certBytes = Convert.FromBase64String(headerValue.ToString());
                    return new X509Certificate2(certBytes);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse certificate from header: {Header}", certHeader);
                }
            }
        }

        // Try to extract from gRPC metadata
        var certMetadata = context.RequestHeaders.Get("x-client-certificate")?.Value;
        if (!string.IsNullOrEmpty(certMetadata))
        {
            try
            {
                var certBytes = Convert.FromBase64String(certMetadata);
                return new X509Certificate2(certBytes);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse certificate from metadata");
            }
        }

        return null;
    }

    private bool VerifyCertificate(X509Certificate2 certificate)
    {
        try
        {
            // Basic validation
            if (!certificate.HasPrivateKey)
            {
                // For client certificates, we don't need private key
                // Just verify it's not expired
                if (certificate.NotAfter < DateTime.UtcNow)
                {
                    _logger.LogWarning("Certificate expired: {Subject}, Expiry: {Expiry}", 
                        certificate.Subject, certificate.NotAfter);
                    return false;
                }

                if (certificate.NotBefore > DateTime.UtcNow)
                {
                    _logger.LogWarning("Certificate not yet valid: {Subject}, ValidFrom: {ValidFrom}", 
                        certificate.Subject, certificate.NotBefore);
                    return false;
                }
            }

            // Additional validation can be added here (e.g., revocation check)
            // For now, basic expiry check is sufficient

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying certificate: {Subject}", certificate.Subject);
            return false;
        }
    }
}
