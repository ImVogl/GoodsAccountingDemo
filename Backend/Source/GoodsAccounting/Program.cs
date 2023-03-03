namespace GoodsAccounting
{
    using Microsoft.OpenApi.Models;

    /// <summary>
    /// Entry class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entry method.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
#if DEBUG
            ConfigureNoCors(builder.Services);
#endif
            RegisterDependencies(builder.Services);
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
#if DEBUG
            app.UseCors("CORS_Policy");
#endif
            // TODO: Add db initialization!
            app.Run();
        }
        
        /// <summary>
        /// Disable CORS control.
        /// </summary>
        /// <param name="serviceCollection">Instance of <see cref="IServiceCollection"/> for web application.</param>
        private static void ConfigureNoCors(IServiceCollection serviceCollection)
        {
            serviceCollection.AddCors(options =>
            {
                options.AddPolicy("CORS_Policy", policyBuilder =>
                {
                    policyBuilder.AllowAnyHeader();
                    policyBuilder.AllowAnyMethod();
                    policyBuilder.AllowAnyOrigin();
                });
            });

            serviceCollection.AddRouting(r => r.SuppressCheckForUnhandledSecurityMetadata = true);
        }

        /// <summary>
        /// Configure swagger
        /// </summary>
        /// <param name="serviceCollection">Instance of <see cref="IServiceCollection"/> for web application.</param>
        private static void ConfigureSwagger(IServiceCollection serviceCollection)
        {
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
                option.AddSecurityDefinition("Bearer", GenerateSecurityScheme());
                option.AddSecurityRequirement(GenerateSecurityRequirement());
            });

            serviceCollection.AddSwaggerGenNewtonsoftSupport();

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
                Name = "JWT authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Authorization for goods accounting web application"
            };
        }

        /// <summary>
        /// Registration types.
        /// </summary>
        /// <param name="serviceCollection">Instance of <see cref="IServiceCollection"/> for web application.</param>
        private static void RegisterDependencies(IServiceCollection serviceCollection){}
    }
}