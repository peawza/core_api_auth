using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Utils.Extensions
{
    public class ActionExceptionFilter : IExceptionFilter
    {
        private readonly ILogger logger;

        public ActionExceptionFilter(
            ILogger<ActionExceptionFilter> logger)
        {
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var controller = (context.ActionDescriptor as ControllerActionDescriptor);
            DateTime exceptionDate = Utils.Extensions.IOUtil.GetCurrentDateTime;
            this.logger.LogError("[{0:dd/MM/yyyy HH:mm:ss}][{1}, {2}] {3}",
                    exceptionDate,
                    controller?.ControllerName,
                    controller?.ActionName,
                    context.Exception.Message);

            context.HttpContext.Response.StatusCode = 500;
            context.Result = new JsonResult(new
            {
                ExceptionDate = exceptionDate
            });
        }
    }
}