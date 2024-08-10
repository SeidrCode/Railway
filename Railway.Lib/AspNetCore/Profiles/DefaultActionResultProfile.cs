using Microsoft.AspNetCore.Mvc;
using Railway.Lib.AspNetCore.Contexts;
using Railway.Lib.AspNetCore.CustomObjectResults;
using Railway.Lib.Constants;

namespace Railway.Lib.AspNetCore.Profiles;

public class DefaultActionResultProfile : IActionResultProfile
{
    public ActionResult TransformToSuccessActionResult(ActionResultProfileSuccessContext context)
    {
        return new ObjectResult(null) { StatusCode = 200 };
    }

    public ActionResult<T> TransformToSuccessActionResult<T>(ActionResultProfileSuccessContext<T> context)
    {
        return new ObjectResult(context.Result.ValueOrDefault) { StatusCode = 200 };
    }

    public ActionResult TransformToFailureActionResult(ActionResultProfileFailureContext context)
    {
        var errorType = context.Error.GetErrorTypeFromMetadata();

        var isBusinessError = errorType == ErrorTypes.BusinessError;

        return isBusinessError
            ? new BadRequestObjectResult(context.Error.ToString())
            : new InternalServerErrorObjectResult(context.Error.ToString());
    }
}
