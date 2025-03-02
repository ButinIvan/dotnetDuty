using Amazon.S3;
using Amazon.S3.Model;
using dotnetWebApi.Entities;

namespace dotnetWebApi.Data;

public class S3DocumentService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName = "dotnet-duty-bucket";

    public S3DocumentService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }
    
    public async Task UploadDocumentAsync(Document document)
    {
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = document.Title,
            ContentBody = document.Body,
            ContentType = "text/plain",
            Headers = {ContentLength = document.Body!.Length}
        };

        try
        {
            await _s3Client.PutObjectAsync(request);
            Console.WriteLine($"Uploading to bucket: {_bucketName}");
        }
        catch (AmazonS3Exception ex)
        {
            // Логирование или обработка ошибок
            throw new Exception("Ошибка при загрузке документа в S3.", ex);
        }
    }

    public async Task<string?> DownloadDocumentAsync(string documentTitle)
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = documentTitle
        };

        try
        {
            using var response = await _s3Client.GetObjectAsync(request);
            using var reader = new StreamReader(response.ResponseStream);
            return await reader.ReadToEndAsync();
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Логирование ошибки отсутствующего документа
            return null;
        }
        catch (Exception ex)
        {
            // Логирование других ошибок
            throw new Exception("Ошибка при скачивании документа из S3.", ex);
        }
    }
    
    public async Task UpdateDocumentAsync(Document document)
    {
        var request = new GetObjectMetadataRequest
        {
            BucketName = _bucketName,
            Key = document.Title
        };

        try
        {
            await _s3Client.GetObjectMetadataAsync(request); // Проверка, существует ли документ
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            throw new KeyNotFoundException("Документ не найден в S3.");
        }

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = document.Title,
            ContentBody = document.Body, // Или Stream, если это необходимо
            ContentType = "text/plain"
        };

        try
        {
            await _s3Client.PutObjectAsync(putRequest); // Обновление документа
        }
        catch (AmazonS3Exception ex)
        {
            throw new Exception("Ошибка при обновлении документа в S3.", ex);
        }
    }

    public async Task DeleteDocumentAsync(string documentTitle)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = documentTitle
        };

        try
        {
            await _s3Client.DeleteObjectAsync(request); // Удаление документа
        }
        catch (AmazonS3Exception ex)
        {
            throw new Exception("Ошибка при удалении документа из S3.", ex);
        }
    }

    public async Task<List<string>> ListDocumentsAsync()
    {
        var request = new ListObjectsV2Request
        {
            BucketName = _bucketName
        };

        try
        {
            var response = await _s3Client.ListObjectsV2Async(request);
            return response.S3Objects.Select(o => o.Key).ToList();
        }
        catch (AmazonS3Exception ex)
        {
            throw new Exception("Ошибка при получении списка документов из S3.", ex);
        }
    }
}
