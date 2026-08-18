namespace ScrappyCoco.Models;
public class ApiValidationException : Exception
{
    public int StatusCode { get; }
    public Dictionary<string, string[]> Errors { get; }

    public ApiValidationException(
        int statusCode,
        Dictionary<string, string[]> errors,
        string? message = null
    )
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors;
    }
}