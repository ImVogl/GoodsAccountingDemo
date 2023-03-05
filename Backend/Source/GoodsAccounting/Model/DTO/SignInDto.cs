using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// User authorization model.
/// </summary>
public class SignInDto
{
    /// <summary>
    /// Get or set user's name.
    /// </summary>
    [JsonRequired]
    [JsonProperty("login")]
    public string UserLogin { get; set; } = null!;

    /// <summary>
    /// Get or set user's password.
    /// </summary>
    [JsonRequired]
    [JsonProperty("password")]
    public string Password { get; set; } = null!;
}