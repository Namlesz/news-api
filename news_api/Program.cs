using news_api.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureMongoDbConnection();
builder.AddMicrosoftIdentity();
builder.AddJwtBearerAuthentication();

builder.Services.ConfigureMsIdentity();
builder.Services.AddRepositories();
builder.Services.AddLogic();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Services.InitializeRoles();

// if (app.Environment.IsDevelopment())
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
