using dotnetWebApi.Documents.Models;
using dotnetWebApi.Entities;
using dotnetWebApi.Interfaces;
using dotnetWebApi.Models;

namespace dotnetWebApi.Documents.Services;

public class DocumentService(IDocumentRepository documentRepository, IAccountRepository accountRepository, IS3Repository s3Repository)
{
    private readonly IDocumentRepository _documentRepository = documentRepository;
    private readonly IAccountRepository _accountRepository = accountRepository;
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

    public async Task<(bool Success, string Message)> EditDocumentAsync(Guid userId, Guid documentId, string content)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Document not found");

        if (document.OwnerId != userId) return (false, "Access denied");
        if (document.IsFinished) return (false, "Document is finished, you can't edit this document");
        
        var newS3Path = await _s3Repository.UploadDocumentAsync(userId, content);
        await _s3Repository.DeleteDocumentAsync(document.S3Path);
        document.UpdateS3Path(newS3Path);
        document.UpdateLastEdited();
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

    public async Task<(bool Success, object? NewReviewer, string Message)> AddReviewerAsync(Guid documentId,
        Guid userId, string userName, string role = "Reviewer")
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, null, "Document not found");
 
        if (document.OwnerId != userId) return (false, null, "Access denied");
 
        var user = await _accountRepository.GetByUserNameAsync(userName);
        if (user == null) return (false, null, "User not found");
        
        var isReviewer = await _documentRepository.GetReviewerAsync(documentId, user.Id);
        if (isReviewer != null) return (false, null, "User is already reviewer");
        await _documentRepository.AddReviewerAsync(documentId, user.Id, role);
        
        var newReviewer = new Reviewer(documentId, user.Id, role);
        return (true, newReviewer, "New reviewer has been added");
    }

    public async Task<(bool Success, string Message)> DeleteReviewerAsync(Guid documentId, Guid ownerId, Guid userId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Access denied");
        
        if (document.OwnerId != ownerId) return (false, "Access denied");
        
        var reviewer = await _documentRepository.GetReviewerAsync(documentId, userId);
        if (reviewer == null) return (false, "No such reviewer");
        if (reviewer.UserId == ownerId) return (false, "You can not remove yourself");
        
        await _documentRepository.RemoveReviewerAsync(reviewer);
        return (true, "Reviewer has been deleted");
    }

    public async Task<List<Reviewer>> GetAllReviewersAsync(Guid documentId)
    {
        var reviewers = await _documentRepository.GetAllReviewersAsync(documentId);
        return reviewers;
    }

    public async Task<(bool Success, string Message)> AddCommentAsync(Guid documentId, Guid userId, string content)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Document not found");
        
        var s3Path = await _s3Repository.UploadDocumentAsync(userId, content);
        
        var isReviewer = await _documentRepository.IsReviewerAsync(documentId, userId);
        if (document.OwnerId != userId && isReviewer != true) return (false, "Access denied");
        var reviewer = await _documentRepository.GetReviewerAsync(documentId, userId);

        var comment = new Comment(documentId, reviewer.Id, s3Path);
        await _documentRepository.AddCommentAsync(comment);
        return (true, "Comment has been added");
    }

    public async Task<List<Comment>> GetAllCommentsAsync(Guid documentId)
    {
        var comments = await _documentRepository.GetAllCommentsAsync(documentId);
        return comments;
    }

    public async Task<(bool Success, string Message)> DeleteCommentAsync(Guid documentId, Guid userId, Guid commentId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Document not found");
        
        if (document.OwnerId != userId) return (false, "Access denied");
        
        var comment = await _documentRepository.GetCommentByIdAsync(commentId);
        if (comment == null) return (false, "Comment has been deleted");
        await _documentRepository.DeleteCommentAsync(comment);
        return (true, "Comment has been deleted");
    }
}