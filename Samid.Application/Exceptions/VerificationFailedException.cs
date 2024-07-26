namespace Samid.Application.Exceptions;

public class VerificationFailedException : ApplicationException
{
    public VerificationFailedException(string message) : base(message) { }
}