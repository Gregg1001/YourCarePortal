using Microsoft.AspNetCore.Mvc.Filters;
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class NoCacheAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate, max-age=0, private";
        filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";
        filterContext.HttpContext.Response.Headers["Expires"] = "-1";

        base.OnResultExecuting(filterContext);
    }
}
