namespace RestifyServer.Exceptions;

public class BuisnessLogicException : DomainException
{
    protected BuisnessLogicException(string message) : base(message)
    {
    }

    public BuisnessLogicException() : base()
    {
    }

    public BuisnessLogicException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
