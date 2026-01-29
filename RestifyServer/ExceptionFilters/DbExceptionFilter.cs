using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace RestifyServer.ExceptionFilters;

public sealed class DbExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not DbUpdateException dbEx) return;
        var problem = CreateProblemDetails(dbEx, out var statusCode);
        context.Result = new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
        context.ExceptionHandled = true;
    }
    private static ProblemDetails CreateProblemDetails(
        DbUpdateException ex,
        out int statusCode)
    {
        if (IsUniqueConstraintViolation(ex))
        {
            statusCode = StatusCodes.Status409Conflict;

            return new ProblemDetails
            {
                Status = statusCode,
                Title = "Conflict",
                Detail = "A resource with the same unique value already exists."
            };
        }

        if (IsConstraintViolation(ex))
        {
            statusCode = StatusCodes.Status400BadRequest;

            return new ProblemDetails
            {
                Status = statusCode,
                Title = "Invalid operation",
                Detail = "The operation violates a database constraint."
            };
        }

        statusCode = StatusCodes.Status500InternalServerError;

        return new ProblemDetails
        {
            Status = statusCode,
            Title = "Database error",
            Detail = "An unexpected database error occurred."
        };
    }
    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        => ex.InnerException switch
        {
            PostgresException pg => pg.SqlState == "23505",
            _ => false
        };

    private static bool IsConstraintViolation(DbUpdateException ex)
        => ex.InnerException switch
        {
            PostgresException pg => pg.SqlState == "23503",         // PostgreSQL FK
            _ => false
        };
}
