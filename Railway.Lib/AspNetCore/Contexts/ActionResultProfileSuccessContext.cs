using Railway.Lib.Base;

namespace Railway.Lib.AspNetCore.Contexts;

public record ActionResultProfileSuccessContext(Result Result);

public record ActionResultProfileSuccessContext<T>(Result<T> Result);

