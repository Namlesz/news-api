using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using NewsApp.api.Context;
using NewsApp.api.Interfaces.Logic;
using NewsApp.api.Interfaces.Repositories;
using NewsApp.api.Repositories;
using NewsApp.api.Services;
using Swashbuckle.AspNetCore.Filters;

namespace NewsApp.api.Helpers;

public static class ServiceExtensions
{
    public static void ConfigureMongoDbConnection(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("NewsDatabase"));

        builder.Services.AddSingleton<IMongoDatabase>(sp =>
        {
            var databaseSettings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
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
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IOfficeService, OfficeService>();
        services.AddScoped<IArticleService, ArticleService>();
    }

    public static void InitializeRoles(this IServiceProvider service)
    {
        DatabaseSeed.SeedRoles(service).Wait();
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
            options.SwaggerDoc("v1", new() { Title = "NewsApp.api", Version = "v1" });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "bearer"
                        }
                    },
                    new string[] { }
                }
            });
            options.AddSecurityDefinition("bearer", new()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                              "Enter 'bearer' [space] and then your token in the text input below.\r\n\r\n" +
                              "Example: 'bearer {{token}}'",
            });
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    // ReSharper disable once InconsistentNaming
    public static void ConfigureSwaggerUI(this WebApplication app)
    {
        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "";
            options.SwaggerEndpoint("swagger/v1/swagger.json", "NewsApp.api v1");
        });
    }
}