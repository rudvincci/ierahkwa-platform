using System.Net.Http.Json;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Contracts.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.BlazorWasm.Services;

/// <summary>
/// Service implementation for Identity operations from BlazorWasm.
/// </summary>
public class IdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IdentityService>? _logger;

    public IdentityService(HttpClient httpClient, IConfiguration configuration, ILogger<IdentityService>? logger = null)
    {
        _httpClient = httpClient;
        _logger = logger;
        var baseUrl = configuration.GetValue<string>("ApiGateway:Url") ?? "https://localhost:5000";
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
        }
    }

    #region Basic CRUD Operations

    public async Task<List<IdentityDto>> GetIdentitiesAsync(CancellationToken cancellationToken = default)
    {
        try 
        {
            var result = await _httpClient.GetFromJsonAsync<List<IdentityDto>>("/api/identities", cancellationToken);
            return result ?? new List<IdentityDto>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting identities");
            return new List<IdentityDto>();
        }
    }

    public async Task<IdentityDto?> GetIdentityAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<IdentityDto>($"/api/identities/{id}", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting identity {IdentityId}", id);
            return null;
        }
    }

    public async Task<bool> AddIdentityAsync(AddIdentity command, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/identities", command, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding identity");
            return false;
        }
    }

    public async Task<bool> UpdateIdentityAsync(UpdateIdentityRequest command, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/identities/{command.IdentityId}", command, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating identity {IdentityId}", command.IdentityId);
            return false;
        }
    }

    public async Task<bool> DeleteIdentityAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/identities/{id}", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error deleting identity {IdentityId}", id);
            return false;
        }
    }

    #endregion

    #region Biometric Operations

    public async Task<BiometricEnrollmentResultDto> EnrollBiometricAsync(Guid identityId, BiometricEnrollmentDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/identities/{identityId}/biometrics", request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<BiometricEnrollmentResultDto>(cancellationToken);
                return result ?? new BiometricEnrollmentResultDto(false, null, "Unknown error");
            }
            return new BiometricEnrollmentResultDto(false, null, $"HTTP {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error enrolling biometric for identity {IdentityId}", identityId);
            return new BiometricEnrollmentResultDto(false, null, ex.Message);
        }
    }

    public async Task<BiometricVerificationResultDto> VerifyBiometricAsync(Guid identityId, BiometricVerificationDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/identities/{identityId}/biometrics/verify", request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<BiometricVerificationResultDto>(cancellationToken);
                return result ?? new BiometricVerificationResultDto(false, 0, "Unknown error");
            }
            return new BiometricVerificationResultDto(false, 0, $"HTTP {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error verifying biometric for identity {IdentityId}", identityId);
            return new BiometricVerificationResultDto(false, 0, ex.Message);
        }
    }

    public async Task<List<BiometricInfoDto>> GetBiometricsAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<BiometricInfoDto>>($"/api/identities/{identityId}/biometrics", cancellationToken);
            return result ?? new List<BiometricInfoDto>();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting biometrics for identity {IdentityId}", identityId);
            return new List<BiometricInfoDto>();
        }
    }

    #endregion

    #region Verification Operations

    public async Task<VerificationResultDto> RequestVerificationAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/identities/{identityId}/verification", null, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<VerificationResultDto>(cancellationToken);
                return result ?? new VerificationResultDto(false, null, "Unknown error");
            }
            return new VerificationResultDto(false, null, $"HTTP {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error requesting verification for identity {IdentityId}", identityId);
            return new VerificationResultDto(false, null, ex.Message);
        }
    }

    public async Task<VerificationStatusDto> GetVerificationStatusAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<VerificationStatusDto>($"/api/identities/{identityId}/verification/status", cancellationToken);
            return result ?? new VerificationStatusDto("Unknown", null, null);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error getting verification status for identity {IdentityId}", identityId);
            return new VerificationStatusDto("Error", null, null);
        }
    }

    #endregion

    #region MFA Operations

    public async Task<MfaSetupResultDto> EnableMfaAsync(Guid identityId, string mfaType, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/identities/{identityId}/mfa/enable", new { mfaType }, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MfaSetupResultDto>(cancellationToken);
                return result ?? new MfaSetupResultDto(false, null, null, "Unknown error");
            }
            return new MfaSetupResultDto(false, null, null, $"HTTP {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error enabling MFA for identity {IdentityId}", identityId);
            return new MfaSetupResultDto(false, null, null, ex.Message);
        }
    }

    public async Task<bool> DisableMfaAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/identities/{identityId}/mfa/disable", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error disabling MFA for identity {IdentityId}", identityId);
            return false;
        }
    }

    public async Task<MfaVerificationResultDto> VerifyMfaAsync(Guid identityId, string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/identities/{identityId}/mfa/verify", new { code }, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<MfaVerificationResultDto>(cancellationToken);
                return result ?? new MfaVerificationResultDto(false, "Unknown error");
            }
            return new MfaVerificationResultDto(false, $"HTTP {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error verifying MFA for identity {IdentityId}", identityId);
            return new MfaVerificationResultDto(false, ex.Message);
        }
    }

    #endregion
}
