using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using dotnetWebApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var awsOptions = builder.Configuration.GetSection("AWS");
var credentials = new BasicAWSCredentials(awsOptions["AccessKey"], awsOptions["SecretKey"]);
var config = new AmazonS3Config
{
    ServiceURL = "https://s3.yandexcloud.net",
    ForcePathStyle = true,
    
};
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IAmazonS3>(new AmazonS3Client(credentials, config));
builder.Services.AddSingleton<S3DocumentService>();
//builder.Services.AddAWSService<IAmazonS3>();
//builder.Services.AddSingleton<S3DocumentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();