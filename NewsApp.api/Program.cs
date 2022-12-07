using NewsApp.api.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureMongoDbConnection();
builder.AddMicrosoftIdentity();
builder.AddJwtBearerAuthentication();

builder.Services.ConfigureMsIdentity();
builder.Services.AddRepositories();
builder.Services.AddLogic();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureCors();

var app = builder.Build();

app.Services.InitializeRoles();

app.UseSwagger();
app.ConfigureSwaggerUI();

if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();