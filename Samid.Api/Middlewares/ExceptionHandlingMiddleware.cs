using System.Net;
using System.Text.Json;
using Samid.Api.Results;
using Samid.Domain.Exceptions;
using Samid.Application.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using ApplicationException = System.ApplicationException;

namespace Samid.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionHandlingMiddleware> _logger;

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

  private async Task HandleExceptionAsync(HttpContext context, Exception exception)
  {
    _logger.LogError(exception, exception.Message);

    var (statusCode, result) = exception switch
    {
      ApplicationException => (HttpStatusCode.BadRequest, ApiResult.BadRequest(exception.Message)),
      DomainException => (HttpStatusCode.UnprocessableEntity, ApiResult.Error(exception.Message, (int)HttpStatusCode.UnprocessableEntity)),
      UnauthorizedAccessException => (HttpStatusCode.Unauthorized, ApiResult.Unauthorized(exception.Message)),
      _ => (HttpStatusCode.InternalServerError, ApiResult.InternalServerError("An unexpected error occurred."))
    };

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = (int)statusCode;

    var response = JsonSerializer.Serialize(result);
    await context.Response.WriteAsync(response);
  }
}
