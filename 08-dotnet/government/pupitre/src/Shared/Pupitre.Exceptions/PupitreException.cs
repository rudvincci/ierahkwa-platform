namespace Pupitre.Exceptions;

/// <summary>
/// Base exception for all Pupitre-specific exceptions.
/// </summary>
public abstract class PupitreException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    protected PupitreException(string code, string message, int statusCode = 400, Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
        StatusCode = statusCode;
    }
}

/// <summary>
/// Exception thrown when a requested entity is not found.
/// </summary>
public class EntityNotFoundException : PupitreException
{
    public string EntityType { get; }
    public string EntityId { get; }

    public EntityNotFoundException(string entityType, string entityId)
        : base("ENTITY_NOT_FOUND", $"{entityType} with ID '{entityId}' was not found.", 404)
    {
        EntityType = entityType;
        EntityId = entityId;
    }

    public static EntityNotFoundException For<T>(Guid id) => new(typeof(T).Name, id.ToString());
}

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException : PupitreException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(string message, IDictionary<string, string[]>? errors = null)
        : base("VALIDATION_FAILED", message, 400)
    {
        Errors = errors?.AsReadOnly() ?? new Dictionary<string, string[]>().AsReadOnly();
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : this("One or more validation errors occurred.", errors)
    {
    }
}

/// <summary>
/// Exception thrown when user is not authorized.
/// </summary>
public class UnauthorizedException : PupitreException
{
    public UnauthorizedException(string message = "You are not authorized to perform this action.")
        : base("UNAUTHORIZED", message, 401)
    {
    }
}

/// <summary>
/// Exception thrown when access is forbidden.
/// </summary>
public class ForbiddenException : PupitreException
{
    public ForbiddenException(string message = "Access to this resource is forbidden.")
        : base("FORBIDDEN", message, 403)
    {
    }
}

/// <summary>
/// Exception thrown when a conflict occurs (e.g., duplicate entry).
/// </summary>
public class ConflictException : PupitreException
{
    public ConflictException(string message)
        : base("CONFLICT", message, 409)
    {
    }
}

/// <summary>
/// Exception thrown when a business rule is violated.
/// </summary>
public class BusinessRuleException : PupitreException
{
    public BusinessRuleException(string code, string message)
        : base(code, message, 422)
    {
    }
}
