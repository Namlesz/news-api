using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using news_api.Helpers;
using news_api.Interfaces.Logic;
using news_api.Interfaces.Repositories;
using news_api.Logic;
using news_api.Repositories;

namespace news_api.Settings;

public static class ServiceExtensions
{
    public static void ConfigureMongoDbConnection(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<NewsDatabaseSettings>(builder.Configuration.GetSection("NewsDatabase"));

        builder.Services.AddSingleton<IMongoDatabase>(sp =>
        {
            var databaseSettings = sp.GetRequiredService<IOptions<NewsDatabaseSettings>>().Value;
            var mongoDbClient = new MongoClient(databaseSettings.ConnectionString);
            var mongoDb = mongoDbClient.GetDatabase(databaseSettings.DatabaseName);

            return mongoDb;
        });
    }

    public static void AddMicrosoftIdentity(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
            (
                builder.Configuration["NewsDatabase:ConnectionString"],
                builder.Configuration["NewsDatabase:DatabaseName"]
            )
            .AddDefaultTokenProviders();
    }

    public static void AddJwtBearerAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ??
                                                                        throw new MissingFieldException(
                                                                            "Can't load jwt key.")))
                };
            });
    }

    public static void ConfigureMsIdentity(this IServiceCollection services)
    {
        services.Configure<IdentityOptions>(opts =>
        {
            opts.User.RequireUniqueEmail = true;
            opts.SignIn.RequireConfirmedEmail = true;
        });
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IUsersRepository, UsersRepository>();
        services.AddSingleton<IEditorialOfficesRepository, EditorialOfficesRepository>();
        services.AddSingleton<IArticleRepository, ArticleRepository>();
    }

    public static void AddLogic(this IServiceCollection services)
    {
        services.AddScoped<IApplicationUserLogic, ApplicationUserLogic>();
        services.AddScoped<IEditorialOfficesLogic, EditorialOfficesLogic>();
        services.AddScoped<IArticleLogic, ArticleLogic>();
    }

    public static void InitializeRoles(this IServiceProvider service)
    {
        DataInitializer.SeedRoles(service).Wait();
    }

    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: "AllowAllOrigins",
                policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "news_api", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new()
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345test'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new()
            {
                {
                    new()
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        });
    }

    // ReSharper disable once InconsistentNaming
    public static void ConfigureSwaggerUI(this WebApplication app)
    {
        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "";
            options.SwaggerEndpoint("swagger/v1/swagger.json", "news_api v1");
        });
    }

}