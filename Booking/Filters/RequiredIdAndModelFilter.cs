using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Filters
{
    // This filter checks:
    // - that id (if given as parameter) not is null
    // - that a resulting ViewModel not is null

    public class RequiredIdAndModelFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.TryGetValue("id", out object idValue))
            {
                if (idValue is null) context.Result = new NotFoundResult();
            }
        }
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ViewResult viewResult)
            {
                if (viewResult.Model is null) context.Result = new NotFoundResult();
            }
        }
    }
}
