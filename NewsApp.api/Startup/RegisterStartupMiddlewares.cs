using Microsoft.AspNetCore.Identity;
using NewsApp.api.Context;
using NewsApp.api.Settings;

namespace NewsApp.api.Startup;

public static class RegisterStartupMiddlewares
{
    public static WebApplication SetupMiddleware(this WebApplication app)
    {
        app.SeedRoles().Wait();
        
        app.UseSwagger();
        app.ConfigureSwaggerUi();
        
        if (app.Environment.IsDevelopment())
            app.UseHttpsRedirection();
        
        app.UseCors("AllowAllOrigins");
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();
        return app;
    }

    private static void ConfigureSwaggerUi(this IApplicationBuilder app)
    {
        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = string.Empty;
            options.SwaggerEndpoint("swagger/v1/swagger.json", "NewsApp.api v1");
        });
    }

    private static async Task SeedRoles(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        foreach (var role in Enum.GetNames(typeof(Roles)))
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new() { Name = role });
            }
        }
    }
}