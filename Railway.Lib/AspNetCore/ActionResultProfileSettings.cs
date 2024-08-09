using Railway.Lib.AspNetCore.Profiles;

namespace Railway.Lib.AspNetCore;

public class ActionResultProfileSettings
{
    public IActionResultProfile DefaultProfile = new DefaultActionResultProfile();
}
