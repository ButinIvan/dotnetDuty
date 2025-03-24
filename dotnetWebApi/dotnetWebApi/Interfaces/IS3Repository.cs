namespace dotnetWebApi.Interfaces;

public interface IS3Repository
{
    Task<string> UploadDocumentAsync(Guid userId, string content);
    Task<string> DownloadDocumentAsync(string s3Path);
    Task DeleteAsync(string s3Path);
}