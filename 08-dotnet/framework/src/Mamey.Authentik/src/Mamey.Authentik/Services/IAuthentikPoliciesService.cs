using Mamey.Authentik.Models;

namespace Mamey.Authentik.Services;

/// <summary>
/// Service interface for Authentik Policies API operations.
/// </summary>
public interface IAuthentikPoliciesService
{
    /// <summary>
    /// GET /policies/all/
    /// </summary>
    Task<PaginatedResult<object>> AllListAsync(bool? bindings__isnull = null, bool? promptstage__isnull = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/all/{policy_uuid}/
    /// </summary>
    Task<object?> AllRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /policies/all/{policy_uuid}/
    /// </summary>
    Task<object?> AllDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /policies/all/{policy_uuid}/test/
    /// </summary>
    Task<object?> AllTestCreateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/all/{policy_uuid}/used_by/
    /// </summary>
    Task<object?> AllUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /policies/all/cache_clear/
    /// </summary>
    Task<object?> AllCacheClearCreateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/all/cache_info/
    /// </summary>
    Task<PaginatedResult<object>> AllCacheInfoRetrieveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/all/types/
    /// </summary>
    Task<PaginatedResult<object>> AllTypesListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/bindings/
    /// </summary>
    Task<PaginatedResult<object>> BindingsListAsync(bool? enabled = null, int? order = null, string? policy = null, bool? policy__isnull = null, string? target = null, string? target_in = null, int? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /policies/bindings/
    /// </summary>
    Task<object?> BindingsCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/bindings/{policy_binding_uuid}/
    /// </summary>
    Task<object?> BindingsRetrieveAsync(string policy_binding_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /policies/bindings/{policy_binding_uuid}/
    /// </summary>
    Task<object?> BindingsUpdateAsync(string policy_binding_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /policies/bindings/{policy_binding_uuid}/
    /// </summary>
    Task<object?> BindingsPartialUpdateAsync(string policy_binding_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /policies/bindings/{policy_binding_uuid}/
    /// </summary>
    Task<object?> BindingsDestroyAsync(string policy_binding_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/bindings/{policy_binding_uuid}/used_by/
    /// </summary>
    Task<object?> BindingsUsedByListAsync(string policy_binding_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/dummy/
    /// </summary>
    Task<PaginatedResult<object>> DummyListAsync(string? created = null, bool? execution_logging = null, string? last_updated = null, string? policy_uuid = null, bool? result = null, int? wait_max = null, int? wait_min = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /policies/dummy/
    /// </summary>
    Task<object?> DummyCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/dummy/{policy_uuid}/
    /// </summary>
    Task<object?> DummyRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /policies/dummy/{policy_uuid}/
    /// </summary>
    Task<object?> DummyUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /policies/dummy/{policy_uuid}/
    /// </summary>
    Task<object?> DummyPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /policies/dummy/{policy_uuid}/
    /// </summary>
    Task<object?> DummyDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/dummy/{policy_uuid}/used_by/
    /// </summary>
    Task<object?> DummyUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/event_matcher/
    /// </summary>
    Task<PaginatedResult<object>> EventMatcherListAsync(string? action = null, string? app = null, string? client_ip = null, string? created = null, bool? execution_logging = null, string? last_updated = null, string? model = null, string? policy_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /policies/event_matcher/
    /// </summary>
    Task<object?> EventMatcherCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/event_matcher/{policy_uuid}/
    /// </summary>
    Task<object?> EventMatcherRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /policies/event_matcher/{policy_uuid}/
    /// </summary>
    Task<object?> EventMatcherUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /policies/event_matcher/{policy_uuid}/
    /// </summary>
    Task<object?> EventMatcherPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /policies/event_matcher/{policy_uuid}/
    /// </summary>
    Task<object?> EventMatcherDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/event_matcher/{policy_uuid}/used_by/
    /// </summary>
    Task<object?> EventMatcherUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/expression/
    /// </summary>
    Task<PaginatedResult<object>> ExpressionListAsync(string? created = null, bool? execution_logging = null, string? expression = null, string? last_updated = null, string? policy_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /policies/expression/
    /// </summary>
    Task<object?> ExpressionCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/expression/{policy_uuid}/
    /// </summary>
    Task<object?> ExpressionRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /policies/expression/{policy_uuid}/
    /// </summary>
    Task<object?> ExpressionUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /policies/expression/{policy_uuid}/
    /// </summary>
    Task<object?> ExpressionPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /policies/expression/{policy_uuid}/
    /// </summary>
    Task<object?> ExpressionDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/expression/{policy_uuid}/used_by/
    /// </summary>
    Task<object?> ExpressionUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/geoip/
    /// </summary>
    Task<PaginatedResult<object>> GeoipListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /policies/geoip/
    /// </summary>
    Task<object?> GeoipCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/geoip/{policy_uuid}/
    /// </summary>
    Task<object?> GeoipRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /policies/geoip/{policy_uuid}/
    /// </summary>
    Task<object?> GeoipUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /policies/geoip/{policy_uuid}/
    /// </summary>
    Task<object?> GeoipPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /policies/geoip/{policy_uuid}/
    /// </summary>
    Task<object?> GeoipDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/geoip/{policy_uuid}/used_by/
    /// </summary>
    Task<object?> GeoipUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/geoip_iso3166/
    /// </summary>
    Task<PaginatedResult<object>> GeoipIso3166ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/password/
    /// </summary>
    Task<PaginatedResult<object>> PasswordListAsync(int? amount_digits = null, int? amount_lowercase = null, int? amount_symbols = null, int? amount_uppercase = null, bool? check_have_i_been_pwned = null, bool? check_static_rules = null, bool? check_zxcvbn = null, string? created = null, string? error_message = null, bool? execution_logging = null, int? hibp_allowed_count = null, string? last_updated = null, int? length_min = null, string? password_field = null, string? policy_uuid = null, string? symbol_charset = null, int? zxcvbn_score_threshold = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /policies/password/
    /// </summary>
    Task<object?> PasswordCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/password/{policy_uuid}/
    /// </summary>
    Task<object?> PasswordRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /policies/password/{policy_uuid}/
    /// </summary>
    Task<object?> PasswordUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /policies/password/{policy_uuid}/
    /// </summary>
    Task<object?> PasswordPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /policies/password/{policy_uuid}/
    /// </summary>
    Task<object?> PasswordDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/password/{policy_uuid}/used_by/
    /// </summary>
    Task<object?> PasswordUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/password_expiry/
    /// </summary>
    Task<PaginatedResult<object>> PasswordExpiryListAsync(string? created = null, int? days = null, bool? deny_only = null, bool? execution_logging = null, string? last_updated = null, string? policy_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /policies/password_expiry/
    /// </summary>
    Task<object?> PasswordExpiryCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/password_expiry/{policy_uuid}/
    /// </summary>
    Task<object?> PasswordExpiryRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /policies/password_expiry/{policy_uuid}/
    /// </summary>
    Task<object?> PasswordExpiryUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /policies/password_expiry/{policy_uuid}/
    /// </summary>
    Task<object?> PasswordExpiryPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /policies/password_expiry/{policy_uuid}/
    /// </summary>
    Task<object?> PasswordExpiryDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/password_expiry/{policy_uuid}/used_by/
    /// </summary>
    Task<object?> PasswordExpiryUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/reputation/
    /// </summary>
    Task<PaginatedResult<object>> ReputationListAsync(bool? check_ip = null, bool? check_username = null, string? created = null, bool? execution_logging = null, string? last_updated = null, string? policy_uuid = null, int? threshold = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /policies/reputation/
    /// </summary>
    Task<object?> ReputationCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/reputation/{policy_uuid}/
    /// </summary>
    Task<object?> ReputationRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /policies/reputation/{policy_uuid}/
    /// </summary>
    Task<object?> ReputationUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /policies/reputation/{policy_uuid}/
    /// </summary>
    Task<object?> ReputationPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /policies/reputation/{policy_uuid}/
    /// </summary>
    Task<object?> ReputationDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/reputation/{policy_uuid}/used_by/
    /// </summary>
    Task<object?> ReputationUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/reputation/scores/
    /// </summary>
    Task<PaginatedResult<object>> ReputationScoresListAsync(string? identifier = null, string? identifier_in = null, string? ip = null, int? score = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/reputation/scores/{reputation_uuid}/
    /// </summary>
    Task<object?> ReputationScoresRetrieveAsync(string reputation_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /policies/reputation/scores/{reputation_uuid}/
    /// </summary>
    Task<object?> ReputationScoresDestroyAsync(string reputation_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/reputation/scores/{reputation_uuid}/used_by/
    /// </summary>
    Task<object?> ReputationScoresUsedByListAsync(string reputation_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/unique_password/
    /// </summary>
    Task<PaginatedResult<object>> UniquePasswordListAsync(string? created = null, bool? execution_logging = null, string? last_updated = null, int? num_historical_passwords = null, string? password_field = null, string? policy_uuid = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// POST /policies/unique_password/
    /// </summary>
    Task<object?> UniquePasswordCreateAsync(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/unique_password/{policy_uuid}/
    /// </summary>
    Task<object?> UniquePasswordRetrieveAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// PUT /policies/unique_password/{policy_uuid}/
    /// </summary>
    Task<object?> UniquePasswordUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// PATCH /policies/unique_password/{policy_uuid}/
    /// </summary>
    Task<object?> UniquePasswordPartialUpdateAsync(string policy_uuid, object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// DELETE /policies/unique_password/{policy_uuid}/
    /// </summary>
    Task<object?> UniquePasswordDestroyAsync(string policy_uuid, CancellationToken cancellationToken = default);

    /// <summary>
    /// GET /policies/unique_password/{policy_uuid}/used_by/
    /// </summary>
    Task<object?> UniquePasswordUsedByListAsync(string policy_uuid, CancellationToken cancellationToken = default);

}
