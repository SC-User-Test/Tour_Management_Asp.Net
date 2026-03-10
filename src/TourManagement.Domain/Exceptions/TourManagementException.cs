namespace TourManagement.Domain.Exceptions;

/// <summary>
/// Base exception for tour management domain
/// </summary>
public class TourManagementException : Exception
{
    public TourManagementException() : base()
    {
    }

    public TourManagementException(string message) : base(message)
    {
    }

    public TourManagementException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when an entity is not found
/// </summary>
public class EntityNotFoundException : TourManagementException
{
    public EntityNotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' with key '{key}' was not found.")
    {
    }
}

/// <summary>
/// Exception thrown for validation failures
/// </summary>
public class ValidationException : TourManagementException
{
    public ValidationException(string message) : base(message)
    {
    }
}
