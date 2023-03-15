using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO with information about new registered user.
/// </summary>
public class NewUserDto
{
    /// <summary>
    /// Get or set user's login.
    /// </summary>
    [JsonRequired]
    [JsonProperty("login")]
    public string Login { get; set; } = null!;

    /// <summary>
    /// Get or set user's password.
    /// </summary>
    [JsonRequired]
    [JsonProperty("password")]
    public string Password { get; set; } = null!;
}