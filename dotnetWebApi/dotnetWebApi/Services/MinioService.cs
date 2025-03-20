using System.Text;
using dotnetWebApi.Interfaces;
using Minio;
using Minio.DataModel.Args;

namespace dotnetWebApi.Services;

public class MinioService :IS3Repository
{

    private readonly IMinioClient _minioClient;
    private readonly string _bucketName = "dotnet-duty-bucket";

    public MinioService(string endpoint, string accessKey, string secretKey)
    {
        Console.WriteLine($"🔹 Подключаемся к MinIO: {endpoint}");
        Console.WriteLine($"🔹 AccessKey: {accessKey}");
        Console.WriteLine($"🔹 SecretKey: {new string('*', secretKey.Length)}"); 

        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(false)
            .Build();
    }

    private async Task EnsureBucketExistsAsync()
    {
        Console.WriteLine($"🔹 Проверяем существование бакета: {_bucketName}");
        var exists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));

        if (!exists)
        {
            Console.WriteLine("🔹 Бакет не найден, создаем новый...");
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
            Console.WriteLine("✅ Бакет успешно создан.");
        }
        else
        {
            Console.WriteLine("✅ Бакет уже существует.");
        }
    }

    public async Task<string> UploadDocumentAsync(Guid userId, string content)
    {
        await EnsureBucketExistsAsync();

        string documentName = $"{Guid.NewGuid()}.txt";
        string objectName = $"{userId}/{documentName}";

        byte[] data = Encoding.UTF8.GetBytes(content);
        using var stream = new MemoryStream(data);

        Console.WriteLine($"🔹 Загружаем документ: {objectName} в бакет {_bucketName}");

        try
        {
            var args = new PutObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(objectName)
                .WithObjectSize(data.Length)
                .WithStreamData(stream)
                .WithContentType("text/plain");

            await _minioClient.PutObjectAsync(args);
            Console.WriteLine("✅ Документ успешно загружен в MinIO");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Ошибка загрузки в MinIO: {ex.Message}");
        }
        Console.WriteLine($"🔹 Проверяем, есть ли файл в MinIO после загрузки: {objectName}");
        var objectsList = _minioClient.ListObjectsEnumAsync(new ListObjectsArgs().WithBucket(_bucketName));
        await foreach (var obj in objectsList)
        {
            Console.WriteLine($"📂 Найден файл: {obj.Key}");
        }
        return objectName;
    }

    public async Task<string?> GetDocumentAsync(Guid userId, string documentName)
    {
        var objectName = $"{userId}/{documentName}";
        using var memoryStream = new MemoryStream();
        var args = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithCallbackStream(async stream =>
            {
                await stream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
            });
        await _minioClient.GetObjectAsync(args);
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    public async Task<string> DownloadDocumentAsync(string s3Path)
    {
        using var memoryStream = new MemoryStream();
        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(s3Path)
            .WithCallbackStream(async stream =>
            {
                await stream.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
            }));
    
        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }

    public async Task DeleteDocumentAsync(Guid userId, string documentName)
    {
        var objectName = $"{userId}/{documentName}";
        var args = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName);
        await _minioClient.RemoveObjectAsync(args);
    }

    public async Task DeleteDocumentAsync(string s3Path)
    {
        var objectName = $"{s3Path}";
        var args = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName);
        await _minioClient.RemoveObjectAsync(args);
    }
}
