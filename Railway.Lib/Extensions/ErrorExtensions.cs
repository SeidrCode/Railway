using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Railway.Lib.AspNetCore;
using Railway.Lib.AspNetCore.Profiles;

namespace Railway.Lib.Extensions;

public static class ErrorExtensions
{
    public static IServiceCollection AddApiRailway(this IServiceCollection services)
    {
        services.AddProblemDetails();
        services.AddHttpContextAccessor();
        services.AddTransient<IActionResultProfile, ActionResultProfile>();

        return services;
    }

    public static WebApplication UseErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler(_ => { });

        return app;
    }

    public static WebApplication UseApiRailway(this WebApplication app)
    {
        var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();

        AspNetCoreResult.Setup(x => x.DefaultProfile = new ActionResultProfile(httpContextAccessor, app.Configuration));

        return app;
    }
}
