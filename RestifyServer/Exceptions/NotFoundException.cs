namespace RestifyServer.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(Guid id, Type entityType) : base($"{entityType.Name} was not found, Id: {id}")
    {
    }

    protected NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException() : base()
    {
    }

    public NotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
