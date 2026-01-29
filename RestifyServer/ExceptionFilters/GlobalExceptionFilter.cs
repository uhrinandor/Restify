using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using RestifyServer.Exceptions;
using Serilog;

namespace RestifyServer.ExceptionFilters;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var ex = context.Exception;
        var statusCode = ex switch
        {
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            NotFoundException => StatusCodes.Status404NotFound,
            DbUpdateException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode == 500 ? "Internal Server Error" : "Request failed",
            Detail = ex.Message
        };

        problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        Log.Error(context.HttpContext.TraceIdentifier, ex, problem);
        context.Result = new ObjectResult(problem)
        {
            StatusCode = statusCode,
            ContentTypes = { "application/problem+json" }
        };

        context.ExceptionHandled = true;
    }
}
