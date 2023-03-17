using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO with short user's info.
/// </summary>
public class UserLoginDto
{
    /// <summary>
    /// Get or ser user's identifier.
    /// </summary>
    [JsonRequired]
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>
    /// Get or ser user's login
    /// </summary>
    [JsonRequired]
    [JsonProperty("login")]
    public string Login { get; set; } = null!;
}