using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TaskManager.Api.Middleware;

// Equivalente ao "Exception Filter" do NestJS
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Erro não tratado");

        httpContext.Response.StatusCode = status;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = status == StatusCodes.Status500InternalServerError ? null : exception.Message
        }, cancellationToken);

        return true;
    }
}
