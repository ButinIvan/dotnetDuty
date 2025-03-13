using System.Text;
using Minio;
using Minio.DataModel.Args;

namespace dotnetWebApi.Services;

public class MinioService
{

    private readonly IMinioClient _minioClient;
    private readonly string _bucketName = "dotnet-duty-bucket";

    public MinioService(string endpoint, string accessKey, string secretKey)
    {
        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .WithSSL(false)
            .Build();
    }

    private async Task EnsureBucketExistsAsync()
    {
        var exists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
        if (!exists)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
        }
    }

    public async Task<string> UploadDocumentAsync(Guid userId, string content, string bucketName)
    {
        await EnsureBucketExistsAsync();

        string fileName = $"{Guid.NewGuid()}.txt";
        string objectName = $"{userId}/{fileName}";

        byte[] data = Encoding.UTF8.GetBytes(content);
        using var stream = new MemoryStream(data);

        var args = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithObjectSize(data.Length)
            .WithStreamData(stream)
            .WithContentType("text/plain");
        await _minioClient.PutObjectAsync(args);
        return objectName;
    }
}
