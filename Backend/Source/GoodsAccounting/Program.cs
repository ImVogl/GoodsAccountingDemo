using AutoMapper;
using GoodsAccounting.Services.DataBase;
using GoodsAccounting.Services.Validator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using GoodsAccounting.Model.Config;
using GoodsAccounting.Services;
using GoodsAccounting.Services.SecurityKey;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using GoodsAccounting.Services.TextConverter;
using GoodsAccounting.Services.Password;
using GoodsAccounting.Services.BodyBuilder;
using GoodsAccounting.MapperProfiles;
using Microsoft.AspNetCore.Cors.Infrastructure;
using GoodsAccounting.Services.SnapshotConverter;
using GoodsAccounting.HeaderFilters;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.CookiePolicy;

namespace GoodsAccounting
{

    /// <summary>
    /// Entry class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// CORS policy name.
        /// </summary>
        private const string CorsName = "CORS";

        /// <summary>
        /// Entry method.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            RegisterDependencies(builder.Services);
            ConfigureCors(builder.Services);
            ConfigureSwagger(builder.Services);
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // HTTPS request only force!
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseAuthentication();    // Checking who is connected user.
            app.UseAuthorization();     // Checking what permissions has connected user.
            app.MapControllers();
            var cookiePolicyOptions = new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict, HttpOnly = HttpOnlyPolicy.Always };
            app.UseCookiePolicy(cookiePolicyOptions);
            app.UseCors(CorsName);
            app.Run();
        }
        
        /// <summary>
        /// Configure CORS policy.
        /// </summary>
        /// <param name="serviceCollection">Instance of <see cref="IServiceCollection"/> for web application.</param>
        private static void ConfigureCors(IServiceCollection serviceCollection)
        {
            var provider = serviceCollection.BuildServiceProvider();
            var origin = provider.GetRequiredService<IOptions<BearerSection>>().Value.Origin;
            if (string.IsNullOrWhiteSpace(origin))
                throw new NullReferenceException();

            var policyBuilder = new CorsPolicyBuilder();
            policyBuilder.AllowAnyHeader();
            policyBuilder.AllowAnyMethod();
            policyBuilder.WithOrigins(origin);
            policyBuilder.AllowCredentials();
            serviceCollection.AddCors(options => { options.AddPolicy(CorsName, policyBuilder.Build()); });
            serviceCollection.AddRouting(r => r.SuppressCheckForUnhandledSecurityMetadata = true);
        }

        /// <summary>
        /// Configure swagger
        /// </summary>
        /// <param name="serviceCollection">Instance of <see cref="IServiceCollection"/> for web application.</param>
        private static void ConfigureSwagger(IServiceCollection serviceCollection)
        {
            var provider = serviceCollection.BuildServiceProvider();
            var contact = new OpenApiContact { Email = "knispel.kurt@gmail.com", Name = "Roman" };
            var apiInfo = new OpenApiInfo
            {
                Version = "v1",
                Title = "Goods accounting",
                Description = "It's API for managing of goods in storage",
                Contact = contact
            };

            // Add services to the container.
            serviceCollection.AddControllers().AddNewtonsoftJson();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            serviceCollection.AddEndpointsApiExplorer();

            serviceCollection.AddMvc();
            serviceCollection.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", apiInfo);
                option.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, GenerateSecurityScheme());
                option.AddSecurityRequirement(GenerateSecurityRequirement());
                option.OperationFilter<HeaderFilter>();
            });

            serviceCollection.AddSwaggerGenNewtonsoftSupport();
            var section = provider.GetRequiredService<IOptions<BearerSection>>().Value;
            if (string.IsNullOrWhiteSpace(section.ValidIssuer))
                throw new NullReferenceException(nameof(section.ValidIssuer));

            if (string.IsNullOrWhiteSpace(section.ValidAudience))
                throw new NullReferenceException(nameof(section.ValidAudience));

            var keyExtractor = provider.GetRequiredService<ISecurityKeyExtractor>();
            serviceCollection.AddAuthentication(option =>
                {
                    option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    option.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options => options.TokenValidationParameters = GenerateValidationParameters(section.ValidIssuer, section.ValidAudience, keyExtractor))
                .AddCookie(option =>
                {
                    option.LoginPath = "/signin";
                    option.LogoutPath = "/signout";
                    option.ExpireTimeSpan = TimeSpan.FromDays(15);
                }); 

            serviceCollection.AddAuthentication();
            serviceCollection.AddAuthorization();
        }

        /// <summary>
        /// Create JWT token validation parameters
        /// </summary>
        /// <param name="issuer">Token issuer.</param>
        /// <param name="audience">Token audience.</param>
        /// <param name="extractor">Instance of <see cref="ISecurityKeyExtractor"/>.</param>
        /// <returns>Validation parameters.</returns>
        private static TokenValidationParameters GenerateValidationParameters(string issuer, string audience, ISecurityKeyExtractor extractor)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = extractor.Extract()
            };
        }

        /// <summary>
        /// Creating SecurityRequirement.
        /// </summary>
        /// <returns>Security requirement.</returns>
        private static OpenApiSecurityRequirement GenerateSecurityRequirement()
        {
            return new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                }
            };
        }

        /// <summary>
        /// Generate security authorization scheme.
        /// </summary>
        /// <returns>Security scheme.</returns>
        private static OpenApiSecurityScheme GenerateSecurityScheme()
        {
            return new OpenApiSecurityScheme
            {
                Name = "Authorization",     // Space character is banned there!
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Access token sending format: Bearer [token]"
            };
        }
        
        /// <summary>
        /// Registration types.
        /// </summary>
        /// <param name="serviceCollection">Instance of <see cref="IServiceCollection"/> for web application.</param>
        private static void RegisterDependencies(IServiceCollection serviceCollection)
        {
            serviceCollection.ConfigureOptions<ConfigurationOptionSetup>();
            serviceCollection.AddScoped<IUsersContext>(provider => provider.GetRequiredService<PostgresProxy>());
            serviceCollection.AddScoped<IStorageContext>(provider => provider.GetRequiredService<PostgresProxy>());
            serviceCollection.AddScoped<IAdminStorageContext>(provider => provider.GetRequiredService<PostgresProxy>());
            AddDataBaseContext(serviceCollection);
            serviceCollection.AddScoped<ISecurityKeyExtractor>(_ => new SecurityKeyExtractor());
            serviceCollection.AddScoped<ITextConverter>(_ => new TextConverter());
            serviceCollection.AddScoped<ISnapshotConverter>(provider => new HistorySnapshotConverter(provider.GetRequiredService<IMapper>()));
            serviceCollection.AddScoped<IResponseBodyBuilder>(_ => new ResponseBodyBuilder());
            serviceCollection.AddScoped<IPasswordValidator>(_ => new Validator());
            serviceCollection.AddScoped<IDtoValidator>(_ => new Validator());
            serviceCollection.AddScoped<IPassword>(provider => new PasswordService(provider.GetRequiredService<IPasswordValidator>()));

            serviceCollection.AddAutoMapper(typeof(WorkShiftProfile));
        }

        /// <summary>
        /// Configure data base proxy class and registry one.
        /// </summary>
        /// <param name="serviceCollection">Instance of <see cref="IServiceCollection"/> for web application.</param>
        /// <exception cref="ArgumentNullException"></exception>
        private static void AddDataBaseContext(IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<IEfContext, PostgresProxy>((serviceProvider, options) =>
            {
                var dataBaseOptions = serviceProvider.GetService<IOptions<DataBaseConfig>>()?.Value;
                if (dataBaseOptions == null)
                    throw new ArgumentNullException(nameof(dataBaseOptions));

                options.UseNpgsql(dataBaseOptions.ConnectionString,
                    sqlOptionAction =>
                    {
                        sqlOptionAction.EnableRetryOnFailure(dataBaseOptions.MaxRetryCount);
                        sqlOptionAction.CommandTimeout(dataBaseOptions.CommandTimeout);
                    });

                options.EnableDetailedErrors(dataBaseOptions.EnableDetailedErrors);
            });
        }
    }
}