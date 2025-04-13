using System.Text;
using dotnetWebApi.AuthUsers.Repositories;
using dotnetWebApi.AuthUsers.Services;
using dotnetWebApi.Documents.Repositories;
using dotnetWebApi.Documents.Services;
using dotnetWebApi.Filters;
using dotnetWebApi.Interfaces;
using dotnetWebApi.Middlewares;
using dotnetWebApi.Persistence;
using dotnetWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("CookieAuth", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Cookie,
        Description = "Авторизация через куки (AuthCookie)",
        Name = "Cookie",
        Type = SecuritySchemeType.ApiKey
    });

    c.OperationFilter<ApplyCookieAuthentication>();
});;

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey("AuthCookie"))
                {
                    context.Token = context.Request.Cookies["AuthCookie"];
                    Console.WriteLine($"Токен из Cookie: {context.Token}");
                }
                else
                {
                    Console.WriteLine("Куки AuthCookie не найдены.");
                }
                return Task.CompletedTask;
            }
        };
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
    });


builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddSingleton<AuthService>(o =>
{
    var requiredService = o.GetRequiredService<EnvService>();
    return new AuthService(
    requiredService.GetVariable("ISSUER", "issuer"),
    requiredService.GetVariable("AUDIENCE", "audience"),
    requiredService.GetVariable("SECRETKEY", "secretKey"));
});
builder.Services.AddScoped<DocumentService>();

var connectionString = envService.GetVariable("CONNECTION_STRING");
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(connectionString));

builder.Services.AddScoped<IPasswordHasher, PasswordHashService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IS3Repository>(options =>
{
    var envvService = options.GetRequiredService<EnvService>();
    var endpoint = envvService.GetVariable("MINIO_ENDPOINT", "minio:9000");
    var accessKey = envvService.GetVariable("MINIO_ACCESS_KEY", "minioadmin");
    var secretKey = envvService.GetVariable("MINIO_SECRET_KEY", "minioadmin");

    return new MinioService(endpoint, accessKey, secretKey);
});
    
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync(); 
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//  app.UseHttpsRedirection();
app.UseMiddleware<SwaggerAuthMiddleware>();
app.UseRouting();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();