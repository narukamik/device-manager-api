namespace device_manager_api.Application.Exceptions;

/// <summary>
/// Exception thrown when an invalid JSON Patch operation is attempted
/// </summary>
public class InvalidPatchOperationException : DomainException
{
    public InvalidPatchOperationException(string message) : base(message)
    {
    }
}
