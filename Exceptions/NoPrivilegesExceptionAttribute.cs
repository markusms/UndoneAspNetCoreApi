using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace UndoneAspNetCoreApi.Exceptions
{
    public class NoPrivilegesExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            string exceptionMessage = context.Exception.Message;

            ContentResult contentResult = new ContentResult();
            contentResult.StatusCode = (int)HttpStatusCode.Unauthorized;
            contentResult.Content = exceptionMessage;

            context.Result = contentResult;
            context.ExceptionHandled = true;
        }
    }
}
