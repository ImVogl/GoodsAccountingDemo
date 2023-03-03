namespace GoodsAccounting.Services.BodyBuilder;

/// <summary>
/// Request body builder service.
/// </summary>
public class ResponseBodyBuilder : IResponseBodyBuilder
{
    /// <inheritdoc />
    public Dictionary<string, string> InvalidDtoBuild()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Dictionary<string, string> UnknownBuild()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Dictionary<string, string> TokenBuild(string token)
    {
        return new Dictionary<string, string> { { "token", token } };
    }
}