using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RestifyServer.Exceptions;

namespace RestifyServer.ExceptionFilters;

public class DomainExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is DomainException domainException)
        {
            if (context.Exception is NotFoundException nf)
            {
                context.Result = new NotFoundObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = nf.Message
                });
            }
            else
            {
                context.Result = new ObjectResult(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = domainException.Message
                }
                );
            }
            context.ExceptionHandled = true;
        }
    }
}
