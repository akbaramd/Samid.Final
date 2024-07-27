using System.Net;
using System.Text.Json;
using Samid.Domain.Exceptions;
using ApplicationException = Samid.Application.Exceptions.ApplicationException;

public class ExceptionHandlingMiddleware
{
  private readonly ILogger<ExceptionHandlingMiddleware> _logger;
  private readonly RequestDelegate _next;

  public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      await HandleExceptionAsync(context, ex);
    }
  }

  private Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    _logger.LogError(exception, exception.Message);

    var code = HttpStatusCode.InternalServerError;
    var result = string.Empty;

    switch (exception)
    {
      case ApplicationException _:
        code = HttpStatusCode.BadRequest;
        result = JsonSerializer.Serialize(new { error = exception.Message });
        break;
      case DomainException _:
        code = HttpStatusCode.UnprocessableEntity;
        result = JsonSerializer.Serialize(new { error = exception.Message });
        break;
      case UnauthorizedAccessException _:
        code = HttpStatusCode.Unauthorized;
        result = JsonSerializer.Serialize(new { error = exception.Message });
        break;
      default:
        result = JsonSerializer.Serialize(new { error = "An unexpected error occurred." });
        break;
    }

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = (int)code;
    return context.Response.WriteAsync(result);
  }
}
