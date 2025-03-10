using dotnetWebApi.AuthUsers.Repositories;
using dotnetWebApi.AuthUsers.Services;
using dotnetWebApi.Persistence;
using dotnetWebApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var envService = new EnvService();
builder.Services.AddSingleton(envService);

var dbInitializer = new DbCreation(envService);
await dbInitializer.EnsureDatabaseExistsAsync();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddSingleton<AuthService>(o =>
{
    var requiredService = o.GetRequiredService<EnvService>();
    return new AuthService(
    requiredService.GetVariable("ISSUER", "issuer"),
    requiredService.GetVariable("AUDIENCE", "audience"),
    requiredService.GetVariable("SECRETKEY", "secretKey"));
});

var connectionString = envService.GetVariable("CONNECTION_STRING");
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString));

builder.Services.AddScoped<MinioService>(options =>
{
    var envvService = options.GetRequiredService<EnvService>();
    var endpoint = envvService.GetVariable("MINIO_ENDPOINT", "localhost:9001");
    var accessKey = envvService.GetVariable("MINIO_ACCESS_KEY", "admin");
    var secretKey = envvService.GetVariable("MINIO_SECRET_KEY", "admin123");

    return new MinioService(endpoint, accessKey, secretKey);
});
    
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();