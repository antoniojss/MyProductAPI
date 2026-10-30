using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi;
using MyProductAPI.Communication.Responses;
using MyProductAPI.Exception;
using MyProductAPI.Exception.ExceptionBase;

namespace MyProductAPI.API.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is MyProductAPIException myProductAPIException)
            {
                context.HttpContext.Response.StatusCode = (int)myProductAPIException.GetStatusCode();
                context.Result = new ObjectResult(new ResponseErrorJson(myProductAPIException.GetErrorMessages()));
            }
            else
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesException.UNKNOWN_ERROR));
            }
        }
    }
}
