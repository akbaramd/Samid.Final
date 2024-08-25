using System.Dynamic;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Samid.Api.Results;

/// <summary>
/// Represents the base class for non-generic API responses.
/// </summary>
public sealed class ApiResult
{
    public int StatusCode { get; private set; }
    public string Message { get; private set; }
    public ExpandoObject Properties { get; }

    public ApiResult(int statusCode, string message, object? properties = null)
    {
        StatusCode = statusCode;
        Message = message;
        Properties = new ExpandoObject();

        if (properties != null)
        {
            AddProperties(properties);
        }
    }

    public void AddProperties(object properties)
    {
        var propertiesDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(
            JsonSerializer.Serialize(properties)
        );

        if (propertiesDictionary != null)
        {
            var expandoDict = (IDictionary<string, object?>)Properties;
            foreach (var kvp in propertiesDictionary)
            {
                expandoDict[kvp.Key] = kvp.Value;
            }
        }
    }

    public static ApiResult Success(string message = "Success", int statusCode = StatusCodes.Status200OK, object? properties = null)
    {
        return new ApiResult(statusCode, message, properties);
    }

    public static ApiResult Error(string message = "Error", int statusCode = StatusCodes.Status400BadRequest, object? properties = null)
    {
        return new ApiResult(statusCode, message, properties);
    }

    // Additional static helper methods
    public static ApiResult BadRequest(string message = "Bad Request", object? properties = null)
    {
        return Error(message, StatusCodes.Status400BadRequest, properties);
    }

    public static ApiResult NotFound(string message = "Not Found", object? properties = null)
    {
        return Error(message, StatusCodes.Status404NotFound, properties);
    }

    public static ApiResult Unauthorized(string message = "Unauthorized", object? properties = null)
    {
        return Error(message, StatusCodes.Status401Unauthorized, properties);
    }

    public static ApiResult Forbidden(string message = "Forbidden", object? properties = null)
    {
        return Error(message, StatusCodes.Status403Forbidden, properties);
    }

    public static ApiResult InternalServerError(string message = "Internal Server Error", object? properties = null)
    {
        return Error(message, StatusCodes.Status500InternalServerError, properties);
    }

    // Explicit conversion from ApiResult<T> to ApiResult
    public static implicit operator ApiResult(ApiResult<object> result)
    {
        return new ApiResult(result.StatusCode, result.Message, result.Properties);
    }

    // Explicit conversion from ApiResult to ApiResult<T>
    public static implicit operator ApiResult<object>(ApiResult result)
    {
        return new ApiResult<object>(default!, result.StatusCode, result.Message, result.Properties);
    }
}

/// <summary>
/// Represents an API response with strongly-typed data and additional properties.
/// </summary>
/// <typeparam name="T">The type of the main data to be returned.</typeparam>
public sealed class ApiResult<T>
{
    public int StatusCode { get; private set; }
    public string Message { get; private set; }
    public ExpandoObject Properties { get; }
    public T Data { get; private set; }

    public ApiResult(T data, int statusCode, string message, object? properties = null)
    {
        Data = data;
        StatusCode = statusCode;
        Message = message;
        Properties = new ExpandoObject();

        if (properties != null)
        {
            AddProperties(properties);
        }
    }

    public void AddProperties(object properties)
    {
        var propertiesDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(
            JsonSerializer.Serialize(properties)
        );

        if (propertiesDictionary != null)
        {
            var expandoDict = (IDictionary<string, object?>)Properties;
            foreach (var kvp in propertiesDictionary)
            {
                expandoDict[kvp.Key] = kvp.Value;
            }
        }
    }

    public static ApiResult<T> Success(T data, string message = "Success", int statusCode = StatusCodes.Status200OK, object? properties = null)
    {
        return new ApiResult<T>(data, statusCode, message, properties);
    }

    public static ApiResult<T> Error(string message = "Error", int statusCode = StatusCodes.Status400BadRequest, object? properties = null)
    {
        return new ApiResult<T>(default!, statusCode, message, properties);
    }

    // Specific helper methods
    public static ApiResult<T> BadRequest(string message = "Bad Request", object? properties = null)
    {
        return Error(message, StatusCodes.Status400BadRequest, properties);
    }

    public static ApiResult<T> NotFound(string message = "Not Found", object? properties = null)
    {
        return Error(message, StatusCodes.Status404NotFound, properties);
    }

    public static ApiResult<T> Unauthorized(string message = "Unauthorized", object? properties = null)
    {
        return Error(message, StatusCodes.Status401Unauthorized, properties);
    }

    public static ApiResult<T> Forbidden(string message = "Forbidden", object? properties = null)
    {
        return Error(message, StatusCodes.Status403Forbidden, properties);
    }

    public static ApiResult<T> InternalServerError(string message = "Internal Server Error", object? properties = null)
    {
        return Error(message, StatusCodes.Status500InternalServerError, properties);
    }

    // Explicit conversion from ApiResult<T> to ApiResult
    public static implicit operator ApiResult(ApiResult<T> result)
    {
        return new ApiResult(result.StatusCode, result.Message, result.Properties);
    }

    // Explicit conversion from ApiResult to ApiResult<T>
    public static implicit operator ApiResult<T>(ApiResult result)
    {
        return new ApiResult<T>(default!, result.StatusCode, result.Message, result.Properties);
    }
}
