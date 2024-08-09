using Microsoft.AspNetCore.Mvc;
using Railway.Lib.AspNetCore.Contexts;
using Railway.Lib.AspNetCore.Profiles;
using Railway.Lib.Base;

namespace Railway.Lib.AspNetCore;

public class ActionResultTransformer
{
    public ActionResult Transform(Result result, IActionResultProfile profile)
    {
        return result.IsFailure
            ? profile.TransformToFailureActionResult(new ActionResultProfileFailureContext(result.Error))
            : profile.TransformToSuccessActionResult(new ActionResultProfileSuccessContext(result));
    }

    public ActionResult<T> Transform<T>(Result<T> result, IActionResultProfile profile)
    {
        return result.IsFailure
            ? profile.TransformToFailureActionResult(new ActionResultProfileFailureContext(result.Error))
            : profile.TransformToSuccessActionResult<T>(new ActionResultProfileSuccessContext<T>(result));
    }
}
