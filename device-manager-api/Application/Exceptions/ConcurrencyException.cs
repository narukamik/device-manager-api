namespace device_manager_api.Application.Exceptions;

/// <summary>
/// Exception thrown when a concurrency conflict occurs
/// </summary>
public class ConcurrencyException : DomainException
{
    public ConcurrencyException() 
        : base("The resource was modified by another user. Please refresh and try again.")
    {
    }
    
    public ConcurrencyException(string message) : base(message)
    {
    }
}
