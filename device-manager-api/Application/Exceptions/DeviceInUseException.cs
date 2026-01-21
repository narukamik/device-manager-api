namespace device_manager_api.Application.Exceptions;

/// <summary>
/// Exception thrown when attempting to delete a device that is currently in use
/// </summary>
public class DeviceInUseException : DomainException
{
    public DeviceInUseException() 
        : base("Cannot delete device that is currently in use")
    {
    }
    
    public DeviceInUseException(string message) : base(message)
    {
    }
}
