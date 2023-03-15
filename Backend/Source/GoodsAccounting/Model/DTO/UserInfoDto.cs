using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// DTO with information about user.
/// </summary>
public class UserInfoDto
{
    /// <summary>
    /// Get or set user identifier.
    /// </summary>
    [JsonRequired]
    [JsonProperty("id")]
    public int UserId { get; set; }

    /// <summary>
    /// Get or set value in indicating that user has administrator role.
    /// </summary>
    [JsonRequired]
    [JsonProperty("is_admin")]
    public bool IsAdmin { get; set; }

    /// <summary>
    /// Get or set value is indicating that user has opened working shift.
    /// </summary>
    [JsonRequired]
    [JsonProperty("shift_opened")]
    public bool ShiftIsOpened { get; set; }

    /// <summary>
    /// Get or set displayed name.
    /// </summary>
    [JsonRequired]
    [JsonProperty("name")]
    public string UserDisplayedName { get; set; } = null!;

    /// <summary>
    /// Get or set access token.
    /// </summary>
    [JsonRequired]
    [JsonProperty("token")]
    public string AssessToken { get; set; } = null!;
}