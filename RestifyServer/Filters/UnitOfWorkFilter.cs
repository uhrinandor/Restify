using Microsoft.AspNetCore.Mvc.Filters;
using RestifyServer.Interfaces.Repositories;

namespace RestifyServer.Filters;

public class UnitOfWorkFilter(IUnitOfWork unitOfWork) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();

        if (executedContext.Exception != null || !IsSuccessStatusCode(executedContext.Result)) return;

        await unitOfWork.SaveChangesAsync(context.HttpContext.RequestAborted);
    }

    private static bool IsSuccessStatusCode(object? result)
    {
        if (result is Microsoft.AspNetCore.Mvc.ObjectResult objectResult)
        {
            return objectResult.StatusCode is >= 200 and <= 299;
        }
        return true;
    }
}
