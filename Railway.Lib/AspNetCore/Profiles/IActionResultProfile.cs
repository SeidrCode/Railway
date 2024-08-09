using Microsoft.AspNetCore.Mvc;
using Railway.Lib.AspNetCore.Contexts;

namespace Railway.Lib.AspNetCore.Profiles;

public interface IActionResultProfile
{
    ActionResult TransformToSuccessActionResult(ActionResultProfileSuccessContext context);

    ActionResult<T> TransformToSuccessActionResult<T>(ActionResultProfileSuccessContext<T> context);

    ActionResult TransformToFailureActionResult(ActionResultProfileFailureContext context);
}
