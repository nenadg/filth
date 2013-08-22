using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;

namespace filth.filters
{
    /// <summary>
    /// Filter to check the model state before the controller action is invoked.
    /// </summary>
    public class ModelValidationFilter : ActionFilterAttribute
    {
        
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            
            if (actionContext.ModelState.IsValid == false)
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, actionContext.ModelState);
            }
        }
         

        /*
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var modelState = actionExecutedContext.ActionContext.ModelState;
            if (!modelState.IsValid)
            {
                var errors = modelState
                    .Where(s => s.Value.Errors.Count > 0)
                    .Select(s => new KeyValuePair<string, string>(s.Key, s.Value.Errors.First().ErrorMessage))
                    .ToArray();

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse<KeyValuePair<string, string>[]>(
                    HttpStatusCode.BadRequest,
                    errors
                );
            }
            base.OnActionExecuted(actionExecutedContext);
        }
         */
    }
}