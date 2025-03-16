namespace dotnetWebApi.Interfaces;

public interface IS3Repository
{
    Task<string> UploadDocumentAsync(Guid userId, string content);
    Task<string?> GetDocumentAsync(Guid userId, string documentName);
    Task<string> DownloadDocumentAsync(string s3Path);
    Task DeleteDocumentAsync(Guid userId, string documentName);
    Task DeleteDocumentAsync(string s3Path);
}