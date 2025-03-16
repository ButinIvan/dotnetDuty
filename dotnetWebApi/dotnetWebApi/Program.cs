using System.Text;
using dotnetWebApi.AuthUsers.Repositories;
using dotnetWebApi.AuthUsers.Services;
using dotnetWebApi.Persistence;
using dotnetWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var envService = new EnvService();
builder.Services.AddSingleton(envService);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5100);
    serverOptions.ListenAnyIP(5101);
});

 builder.Services.AddDataProtection()
     .PersistKeysToFileSystem(new DirectoryInfo("/app/keys")) // Используйте том для хранения ключей
     .SetApplicationName("dotnetWebApi");


var dbInitializer = new DbCreation(envService);
await dbInitializer.EnsureDatabaseExistsAsync();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = envService.GetVariable("ISSUER", "issuer"),
            ValidAudience = envService.GetVariable("AUDIENCE", "audience"),
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(envService.GetVariable("SECRETKEY", "secretkey")))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("AuthCookie"))
                {
                    context.Token = context.Request.Cookies["AuthCookie"];
                }

                return Task.CompletedTask;
            }
        };
    });

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
    var accessKey = envvService.GetVariable("MINIO_ACCESS_KEY", "minioadmin");
    var secretKey = envvService.GetVariable("MINIO_SECRET_KEY", "minioadmin");

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