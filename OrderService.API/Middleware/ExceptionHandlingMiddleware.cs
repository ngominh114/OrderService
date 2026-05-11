namespace OrderService.API.Middleware;

using OrderService.Domain.Exceptions;

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
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteSimpleErrorResponseAsync(context, ex);
        }
    }

    private static async Task WriteSimpleErrorResponseAsync(HttpContext context, Exception ex)
    {
        IResult result = ex switch
        {
            NotFoundException => Results.NotFound(ex.Message),
            ConflictException => Results.Conflict(ex.Message),
            ArgumentException => Results.BadRequest(ex.Message),
            _ => Results.Text(ex.Message, statusCode: StatusCodes.Status500InternalServerError)
        };

        await result.ExecuteAsync(context);
    }
}
