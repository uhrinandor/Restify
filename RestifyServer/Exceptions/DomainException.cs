namespace RestifyServer.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    { }

    public DomainException() : base()
    {
    }

    public DomainException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
