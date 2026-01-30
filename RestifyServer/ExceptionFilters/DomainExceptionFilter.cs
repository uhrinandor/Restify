using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RestifyServer.Exceptions;
using Serilog;

namespace RestifyServer.ExceptionFilters;

public class DomainExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is DomainException domainException)
        {
            if (context.Exception is NotFoundException nf)
            {
                ProblemDetails problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = nf.Message
                };
                context.Result = new NotFoundObjectResult(problemDetails);
                Log.Error(context.HttpContext.TraceIdentifier, domainException, problemDetails);
            }
            else
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = domainException.Message
                };
                context.Result = new ObjectResult(problemDetails);
                Log.Error(context.HttpContext.TraceIdentifier, domainException, problemDetails);
            }

            context.ExceptionHandled = true;
        }
    }
}
