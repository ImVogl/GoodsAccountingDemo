using Newtonsoft.Json.Linq;

namespace GoodsAccounting.Services.BodyBuilder;

/// <summary>
/// Request body builder service.
/// </summary>
public class ResponseBodyBuilder : IResponseBodyBuilder
{
    /// <inheritdoc />
    public Dictionary<string, string> InvalidDtoBuild()
    {
        return new Dictionary<string, string> { { "error", "invalidDto" } };
    }

    /// <inheritdoc />
    public Dictionary<string, string> UnknownBuild()
    {
        return new Dictionary<string, string> { { "error", "unknownError" } };
    }

    /// <inheritdoc />
    public Dictionary<string, string> TokenBuild(string token)
    {
        return new Dictionary<string, string> { { "token", token } };
    }
}