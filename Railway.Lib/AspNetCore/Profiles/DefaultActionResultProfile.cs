using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Primitives.Lib.CustomObjectResults;
using Railway.Lib.AspNetCore.Contexts;
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
        var problemDetails = new ProblemDetails();

        var errorType = context.Error.GetErrorTypeFromMetadataOrDefault();

        problemDetails.Title = errorType switch
        {
            ErrorType.BusinessError => "Произошла ошибка бизнес логики.",
            ErrorType.NotFound => "Запрашиваемые данные не найдены.",
            ErrorType.SystemError => "Произошла системная ошибка.",
            _ => throw new NotImplementedException()
        };

        problemDetails.Status = errorType switch
        {
            ErrorType.BusinessError => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.SystemError => StatusCodes.Status500InternalServerError,
            _ => throw new NotImplementedException()
        };

        problemDetails.Extensions.Add("error", context.Error);

        return errorType switch
        {
            ErrorType.BusinessError => new BadRequestObjectResult(problemDetails),
            ErrorType.NotFound => new NotFoundObjectResult(problemDetails),
            ErrorType.SystemError => new InternalServerErrorObjectResult(problemDetails),
            _ => throw new NotImplementedException()
        };
    }
}
