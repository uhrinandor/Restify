using Microsoft.AspNetCore.Mvc;
using RestifyServer.ExceptionFilters;

namespace RestifyServer.Configuration;

public static class ControllerConfigFactory
{
    public static void ConfigureControllers(MvcOptions options)
    {
        // Exception filters
        options.Filters.Add<GlobalExceptionFilter>();
        options.Filters.Add<DbExceptionFilter>();
        options.Filters.Add<DomainExceptionFilter>();

        // Problem details
        options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status400BadRequest));
        options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ProblemDetails), StatusCodes.Status500InternalServerError));
    }
}
