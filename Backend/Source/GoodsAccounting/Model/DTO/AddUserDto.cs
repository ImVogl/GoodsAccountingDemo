using Newtonsoft.Json;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// Add new user data model.
/// </summary>
public class AddUserDto
{
    /// <summary>
    /// Get or set sender id.
    /// </summary>
    [JsonRequired]
    [JsonProperty("id")]
    public int SenderId { get; set; }

    /// <summary>
    /// Get or set new user email.
    /// </summary>
    [JsonRequired]
    [JsonProperty("email")]
    public string Email { get; set; } = null!;

    /// <summary>
    /// Get or set user's telegram login.
    /// </summary>
    [JsonRequired]
    [JsonProperty("telegram")]
    public string Telegram { get; set; } = null!;

    /// <summary>
    /// Get or set new user's name.
    /// </summary>
    [JsonRequired]
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Get or set new user's surname.
    /// </summary>
    [JsonRequired]
    [JsonProperty("surname")]
    public string Surname { get; set; } = null!;

    /// <summary>
    /// Get or set new user's birth day.
    /// </summary>
    [JsonRequired]
    [JsonProperty("date")]
    public DateTime BirthDay { get; set; }
}