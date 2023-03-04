using GoodsAccounting.Model.Config;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;

namespace GoodsAccounting.Services;

/// <summary>
/// Configuration setup proxy.
/// </summary>
public class ConfigurationOptionSetup : IConfigureOptions<JwtSection>
{
    /// <summary>
    /// Instance of <see cref="IConfiguration"/>.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Create new instance of <see cref="ConfigurationOptionSetup"/>.
    /// </summary>
    /// <param name="configuration">Instance of <see cref="IConfiguration"/>.</param>
    public ConfigurationOptionSetup([NotNull] IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
    
    /// <inheritdoc />
    public void Configure(JwtSection options)
    {
        _configuration.GetSection("JWT").Bind(options);
    }
}