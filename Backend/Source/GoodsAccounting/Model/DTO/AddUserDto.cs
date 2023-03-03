using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace GoodsAccounting.Model.DTO;

/// <summary>
/// Add new user data model.
/// </summary>
public class AddUserDto
{
    /// <summary>
    /// Get or set sender id.
    /// </summary>
    [Required]
    [JsonProperty("id")]
    public int SenderId { get; set; }

    /// <summary>
    /// Get or set new user's token.
    /// </summary>
    [Required]
    [JsonProperty("token")]
    public string Token { get; set; } = null!;

    /// <summary>
    /// Get or set new user's name.
    /// </summary>
    [Required]
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Get or set new user's surname.
    /// </summary>
    [Required]
    [JsonProperty("surname")]
    public string Surname { get; set; } = null!;

    /// <summary>
    /// Get or set new user's birth day.
    /// </summary>
    [Required]
    [JsonProperty("date")]
    public DateTime BirthDay { get; set; }
}