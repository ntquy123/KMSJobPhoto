using entities.Common;
using erpsolution.entities.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace erpsolution.api.Attribute
{
    public class ModelStateValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
           // base.OnActionExecuted(context);

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                Dictionary<string, IEnumerable<string>> Errors = new Dictionary<string, IEnumerable<string>>();
                foreach (var state in context.ModelState)
                {
                    Errors.Add(state.Key, state.Value.Errors.Select(i => (i.ErrorMessage.IndexOf(":") != -1 ? i.ErrorMessage.Split(":")[0].Replace(" ", "_").ToUpper() : i.ErrorMessage.Replace(" ", "_").ToUpper())));
                }
                var rs = new HandleResponse<object>(false, "DATA_NOT_VALID", null, Errors, 404);
                context.Result = new OkObjectResult(rs);
            }
        }
    }
}
