using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AutoTrader.Abstractions;

namespace AutoTrader.Api.Filters;

public class ResponseActionFilter: IActionFilter, IOrderedFilter
{
    public int Order { get; set; } = int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        // [SONAR] Não há implementação, pois atualmente não é necessário.
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult
             && objectResult.Value is Response response)
        {
            object result = response.Status == ResponseStatus.Ok ? response.Result : response;
            context.Result = new ObjectResult(result)
            {
                StatusCode = (int)response.Status,
            };
        }
    }

}
