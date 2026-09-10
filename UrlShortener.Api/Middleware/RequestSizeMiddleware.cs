namespace UrlShortener.Api.Middleware;

public class RequestSizeMiddleware
{
    private const long MaxRequestBodySize = 10 * 1024;

    private readonly RequestDelegate _next;

    public RequestSizeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentLength >
            MaxRequestBodySize)
        {
            context.Response.StatusCode =
                StatusCodes.Status413PayloadTooLarge;

            await context.Response.WriteAsJsonAsync(new
            {
                error = "Request body is too large."
            });

            return;
        }

        await _next(context);
    }
}