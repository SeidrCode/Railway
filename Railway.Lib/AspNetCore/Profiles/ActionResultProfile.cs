using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Primitives.Lib.CustomObjectResults;
using Primitives.Lib.Extensions;
using Railway.Lib.AspNetCore.Contexts;
using Railway.Lib.Constants;

namespace Railway.Lib.AspNetCore.Profiles;

public class ActionResultProfile : IActionResultProfile
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public ActionResultProfile(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

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
        var httpContext = _httpContextAccessor.HttpContext;

        var version = GetApiVersionFromContentType(httpContext.Request.ContentType);
        var serviceName = _configuration.GetValue<string>("Application:Name");
        var serviceType = _configuration.GetValue<string>("Application:Type");

        var problemDetails = new ProblemDetails();

        problemDetails.Extensions.Add("serviceName", serviceName);
        problemDetails.Extensions.Add("serviceType", serviceType);
        problemDetails.Extensions.Add("timestamp", DateTime.Now);

        if (!problemDetails.Extensions.ContainsKey("traceId"))
        {
            var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;
            problemDetails.Extensions.Add("traceId", traceId);
        }

        var errorType = context.Error.GetErrorTypeFromMetadata();

        var isBusinessError = errorType == ErrorTypes.BusinessError;

        problemDetails.Title = isBusinessError ? "Произошла ошибка бизнес логики." : "Произошла системная ошибка.";

        if (serviceType == "api")
        {
            problemDetails.Extensions.Add("requestId", httpContext.Request.Headers["x-request-id"].ToString());
            problemDetails.Extensions.Add("systemCode", httpContext.Request.Headers["x-external-system-code"].ToString());
            problemDetails.Extensions.Add("userCode", httpContext.Request.Headers["x-external-user-code"].ToString());
            problemDetails.Extensions.Add("version", version);
            problemDetails.Extensions.Add("method", httpContext.Request.Method);
            problemDetails.Extensions.Add("url", httpContext.Request.GetDisplayUrl());

            if (httpContext.Request.Path.HasValue)
                problemDetails.Extensions.Add("endpoint", httpContext.Request.Path.Value);

            problemDetails.Status = isBusinessError ? StatusCodes.Status400BadRequest : StatusCodes.Status500InternalServerError;
        }

        problemDetails.Extensions.Add("error", context.Error);

        return isBusinessError
            ? new BadRequestObjectResult(problemDetails)
            : new InternalServerErrorObjectResult(problemDetails);
    }

    /// <summary>
    /// Получение версии Api из значения ContentType
    /// </summary>
    private string GetApiVersionFromContentType(string? contentType)
    {
        return string.IsNullOrEmpty(contentType)
            ? string.Empty
            : contentType.Right(3).Replace("v=", "").Replace("=", "");
    }
}
