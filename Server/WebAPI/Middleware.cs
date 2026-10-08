using Entities;

namespace WebAPI.Middleware;

public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException e)            // for 404 errors
        {
            await Write(context, StatusCodes.Status404NotFound, e.Message);
        }
        catch (ArgumentException e)            // used for validation failures
        {
            await Write(context, StatusCodes.Status400BadRequest, e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await Write(context, StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    private static async Task Write(HttpContext context, int status, string message)
    {
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new { error = message });
    }
}