using dotnetWebApi.Documents.Models;
using dotnetWebApi.Entities;
using dotnetWebApi.Interfaces;

namespace dotnetWebApi.Documents.Services;

public class DocumentService(IAccountRepository accountRepository, IDocumentRepository documentRepository, IS3Repository s3Repository)
{
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IDocumentRepository _documentRepository = documentRepository;
    private readonly IS3Repository _s3Repository = s3Repository;

    public async Task<(bool Success, string Message, Guid? DocumentId)> CreateDocumentAsync(Guid userId, string title,
        string content)
    {
        if (string.IsNullOrEmpty(title)) return (false, "Title can not be empty", null);

        var s3Path = await _s3Repository.UploadDocumentAsync(userId, content);
        var documentName = s3Path.Split("/").Last().Split('.').First();
        var documentId = Guid.Parse(documentName);
        
        var document = new Document(documentId, title, userId, s3Path);
        await _documentRepository.AddAsync(document);

        await _documentRepository.AddReviewerAsync(document.Id, userId, "Owner");
        return (true, "Document created", document.Id);
    }

    public async Task<(bool Success, string Message, string Content)> DownloadDocumentAsync(Guid documentId,
        Guid userId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Document not found", "");

        var hasAccess = document.OwnerId == userId || await _documentRepository.IsReviewerAsync(documentId, userId);
        if (!hasAccess) return (false, "Access denied", "");
        var content = await _s3Repository.DownloadDocumentAsync(document.S3Path);
        return (true, "", content);
    }

    public async Task<(bool Success, string Message, string Content, string Role)> GetDocumentAsync(Guid documentId,
        Guid userId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        Console.WriteLine("Can't get document");
        if (document == null) return (false, "Document not found", "", "User");

        if (document.OwnerId == userId)
        {
            var content = await _s3Repository.DownloadDocumentAsync(document.S3Path);
            return (true, "Accessed on", content, "Owner");
        }

        var reviewerRole = await _documentRepository.GetUserRoleAsync(documentId, userId);

        if (reviewerRole == null) return (false, "Access denied", "", "User");
        {
            var content = await _s3Repository.DownloadDocumentAsync(document.S3Path);
            return (true, "Collaborator accessed on.", content, reviewerRole);
        }
    }

    public async Task<string> GetUserRoleAsync(Guid documentId, Guid userId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return "User";

        if (document.OwnerId == userId) return "Owner";
        var role = await _documentRepository.GetUserRoleAsync(documentId, userId);
        return role ?? "User";
    }

    public async Task<(bool Success, object? NewReviewer, string Message)> AddReviewerAsync(Guid documentId,
        Guid ownerId, string userName, string role = "Reviewer")
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, null, "Document not found");

        if (document.OwnerId != ownerId) return (false, null, "Access denied");

        var user = await _accountRepository.GetByUserNameAsync(userName);
        if (user == null) return (false, null, "User not found");

        var isReviewer = await _documentRepository.GetReviewerAsync(documentId, user.Id);
        if (isReviewer != null) return (false, null, "User is already reviewer");
        
        await _documentRepository.AddReviewerAsync(documentId, user.Id, role);

        var newReviewer = new
        {
            ReviewerId = user.Id,
            ReviewerUsername = user.UserName,
            ReviewerRole = role
        };

        return (true, newReviewer, "New Reviewer has been added");
    }

    public async Task<(bool Success, string Message)> UpdateDocumentSettingsAsync(Guid documentId, Guid userId, UpdateDocumentSettingsRequest request)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Document not found");

        if (document.OwnerId != userId) return (false, "Access denied");
        
        document.UpdateTitle(request.Title);
        document.SetFinished(request.IsFinished);

        await _documentRepository.UpdateAsync(document);

        return (true, "Document updated");
    }
    
    public async Task<(bool Success, string Message)> DeleteDocumentAsync(Guid documentId, Guid userId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Document not found.");

        if (document.OwnerId != userId) return (false, "Access denied.");

        await _s3Repository.DeleteDocumentAsync(document.S3Path);

        await _documentRepository.RemoveReviewersAsync(documentId);

        await _documentRepository.DeleteAsync(documentId);

        return (true, "Document deleted successfully.");
    }

    public async Task<List<Document>> GetUserDocumentsAsync(Guid userId)
    {
        var documents = await _documentRepository.GetUserDocumentsAsync(userId);
        return documents;
    }
}