using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Booking.Filters
{
    // This filter checks:
    // - that a property (if given as parameter) not is null.
    // - that a resulting ViewModel not is null


    public class RequiredIdAndModelFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Another way to look at property:
            // if (context.RouteData.Values["id] == null)

            if (context.ActionArguments.TryGetValue("id", out object idValue))
            {
                // Property found, test if null
                if (idValue is null) context.Result = new NotFoundResult();
            }
            else
            {
                // No property found
                context.Result = new NotFoundResult();
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
