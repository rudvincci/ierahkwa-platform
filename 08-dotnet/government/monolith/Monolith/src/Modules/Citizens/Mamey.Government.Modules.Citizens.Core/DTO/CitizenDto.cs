using System;

namespace Mamey.Government.Modules.Citizens.Core.DTO;

public record CitizenDto(
    Guid Id,
    Guid TenantId,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    string Status,
    string? Email,
    string? Phone,
    string? Address,
    string? PhotoUrl,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record CitizenSummaryDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Status,
    bool IsActive);

public record CitizenStatusHistoryDto(
    string FromStatus,
    string ToStatus,
    DateTime ChangedAt,
    string? ChangedBy,
    string? Reason);
