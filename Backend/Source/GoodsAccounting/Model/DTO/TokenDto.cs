using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO with token information.
/// </summary>
public class TokenDto
{
    /// <summary>
    /// Get or set access JWT.
    /// </summary>
    [JsonRequired]
    [JsonProperty("token")]
    public string Token { get; set; } = null!;

    /// <summary>
    /// Get or set expired time.
    /// </summary>
    [JsonRequired]
    [JsonProperty("expired")]
    public DateTime ExpiredTime { get; set; }
}