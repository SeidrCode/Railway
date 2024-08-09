using Railway.Lib.Base;

namespace Railway.Lib.AspNetCore.Contexts;

public class ActionResultProfileFailureContext
{
    public ActionResultProfileFailureContext(Error error)
    {
        Error = error;
    }

    public Error Error { get; set; }
}
