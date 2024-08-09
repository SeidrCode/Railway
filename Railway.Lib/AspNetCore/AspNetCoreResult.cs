namespace Railway.Lib.AspNetCore;

public static class AspNetCoreResult
{
    internal static ActionResultProfileSettings Settings { get; private set; }

    static AspNetCoreResult()
    {
        Settings = new ActionResultProfileSettings();
    }

    /// <summary>
    /// Setup global settings
    /// </summary>
    public static void Setup(Action<ActionResultProfileSettings> setupFunc)
    {
        var settingsBuilder = new ActionResultProfileSettings();
        setupFunc(settingsBuilder);

        Settings = settingsBuilder;
    }
}
