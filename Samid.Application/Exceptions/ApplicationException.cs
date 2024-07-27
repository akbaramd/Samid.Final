namespace Samid.Application.Exceptions;

public abstract class ApplicationException : Exception
{
  protected ApplicationException(string message, int statusCode = 400) : base(message)
  {
    StatusCode = statusCode;
  }

  public int StatusCode { get; }
}
