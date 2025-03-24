using dotnetWebApi.Documents.Models;
using dotnetWebApi.Entities;
using dotnetWebApi.Interfaces;
using dotnetWebApi.Models;

namespace dotnetWebApi.Documents.Services;

public class DocumentService(IDocumentRepository documentRepository, IAccountRepository accountRepository, IS3Repository s3Repository, ICommentRepository commentRepository)
{
    // Here and everywhere else where primary constructors are used, it's redundant to duplicate fields,
    // you could just use parameters from the constructor, documentRepository, etc.
    private readonly IDocumentRepository _documentRepository = documentRepository;
    private readonly IAccountRepository _accountRepository = accountRepository;
    private readonly IS3Repository _s3Repository = s3Repository;
    private readonly ICommentRepository _commentRepository = commentRepository;

    // Here and everywhere else,
    // it's better to specify object models to wrap inputs and outputs with relevant names to improve readability.
    public async Task<(bool Success, string Message, Guid? DocumentId)> CreateDocumentAsync(Guid userId, string title,
        string content)
    {
        if (string.IsNullOrEmpty(title)) return (false, "Title can not be empty", null);

        var s3Path = await _s3Repository.UploadDocumentAsync(userId, content);
        // Last() and First() could throw errors, no handling of this
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
            return (true, "Reviewer accessed on.", content, reviewerRole);
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
        await _s3Repository.DeleteAsync(document.S3Path);
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

        await _s3Repository.DeleteAsync(document.S3Path);

        await _documentRepository.RemoveReviewersAsync(documentId);

        await _documentRepository.DeleteAsync(documentId);

        return (true, "Document deleted successfully.");
    }

    public async Task<List<Guid>> GetUserDocumentsAsync(Guid userId)
    {
        var documents = await _documentRepository.GetUserDocumentsAsync(userId);
        var documentIds = documents.Select(document => document.Id).ToList();
        return documentIds;
    }

    public async Task<(bool Success, Guid AddedReviewerUserId, string Message)> AddReviewerAsync(Guid documentId,
        Guid userId, string userName, string role = "Reviewer")
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, Guid.Empty, "Document not found");
 
        if (document.OwnerId != userId) return (false, Guid.Empty, "Access denied");
 
        var user = await _accountRepository.GetByUserNameAsync(userName);
        if (user == null) return (false, Guid.Empty, "User not found");
        
        var isReviewer = await _documentRepository.GetReviewerAsync(documentId, user.Id);
        if (isReviewer != null) return (false, Guid.Empty, "User is already reviewer");
        await _documentRepository.AddReviewerAsync(documentId, user.Id, role);
        
        return (true, userId, "New reviewer has been added");
    }

    public async Task<(bool Success, string Message)> DeleteReviewerAsync(Guid documentId, Guid ownerId, string userName)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Access denied");
        
        if (document.OwnerId != ownerId) return (false, "Access denied");
        
        var user = await _accountRepository.GetByUserNameAsync(userName);
        if (user == null) return (false, "User not found");
        
        var reviewer = await _documentRepository.GetReviewerAsync(documentId, user.Id);
        if (reviewer == null) return (false, "No such reviewer");
        if (reviewer.UserId == ownerId) return (false, "You can not remove yourself");
        
        await _documentRepository.RemoveReviewerAsync(reviewer);
        return (true, "Reviewer has been deleted");
    }

    public async Task<(bool Success, string Message, List<string>)> GetAllReviewersAsync(Guid userId, Guid documentId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Document not found", []);
        if (document.OwnerId != userId) return (false, "Access denied", []);
        
        var users = new List<string>();
        var reviewers = await _documentRepository.GetReviewersAsync(documentId);
        var reviewersIds = reviewers.Select(r => r.UserId).ToList();
        foreach (var id in reviewersIds)
        {
            var user = await _accountRepository.GetByIdAsync(id);
            if (user == null) continue;
            users.Add(user.UserName);
        }
        return (true, "Showing all reviewers", users);
    }

    public async Task<(bool Success, string Message)> AddCommentAsync(Guid documentId, Guid userId, string content)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Document not found");
        
        var s3Path = await _s3Repository.UploadDocumentAsync(userId, content);
        
        var isReviewer = await _documentRepository.IsReviewerAsync(documentId, userId);
        if (document.OwnerId != userId && isReviewer != true) return (false, "Access denied");
        var reviewer = await _documentRepository.GetReviewerAsync(documentId, userId);
        if (reviewer == null) return (false, "No such reviewer");

        var comment = new Comment(documentId, reviewer.Id, s3Path);
        await _commentRepository.AddCommentAsync(comment);
        return (true, "Comment has been added");
    }

    public async Task<(bool Success, string Message, string Content)> GetCommentAsync(Guid commentId)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(commentId);
        if (comment == null) return (false, "Comment not found", "");
        var content = await _s3Repository.DownloadDocumentAsync(comment.S3Path);
        return (true, "Comment is found", content);
    }

    public async Task<(bool Success, string Message, List<Guid> Content)> GetAllCommentsAsync(Guid ownerId, Guid documentId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Document not found", []);
        if (document.OwnerId != ownerId) return (false, "Access denied", []);
        
        var comments = await _commentRepository.GetAllCommentsAsync(documentId);
        var commentsIds = comments.Select(c => c.Id).ToList();
        
        return (true, "Showing all comments", commentsIds);
    }

    public async Task<(bool Success, string Message)> DeleteCommentAsync(Guid documentId, Guid userId, Guid commentId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Document not found");
        
        if (document.OwnerId != userId) return (false, "Access denied");
        
        var comment = await _commentRepository.GetCommentByIdAsync(commentId);
        if (comment == null) return (false, "Comment has been deleted");
        await _s3Repository.DeleteAsync(comment.S3Path);
        await _commentRepository.DeleteCommentAsync(comment);
        return (true, "Comment has been deleted");
    }

    public async Task<(bool Success, string Message)> DeleteAllCommentsAsync(Guid documentId, Guid ownerId)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Document not found");
        
        if (document.OwnerId != ownerId) return (false, "Access denied");
        
        var comments = await _commentRepository.GetAllCommentsAsync(documentId);
        foreach (var comment in comments)
        {
            await _commentRepository.DeleteCommentAsync(comment);
        }
        return (true, "Comments have been deleted");
    }

    public async Task<(bool Success, string Message, List<string> Content)> GetAllReviewerCommentsAsync(Guid ownerId, Guid documentId, string userName)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Document not found", []);
        
        if (document.OwnerId != ownerId) return (false, "Access denied", []);

        var user = await _accountRepository.GetByUserNameAsync(userName);
        if (user == null) return (false, "No such user", []);
        
        var reviewer = await _documentRepository.GetReviewerAsync(documentId, user.Id);
        if (reviewer == null) return (false, "No such reviewer", []);
        
        var reviewerComments = await _commentRepository.GetAllReviewerCommentsAsync(documentId, reviewer.Id);
        var comments = new List<string>();
        foreach (var comment in reviewerComments)
        {
            var content = await _s3Repository.DownloadDocumentAsync(comment.S3Path);
            comments.Add(content);
        }
        return (true, "Reviewer comments has been found", comments);
    }

    public async Task<(bool Success, string Message)> DeleteAllReviewerCommentsAsync(Guid ownerId, Guid documentId, string userName)
    {
        var document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return (false, "Document not found");
        
        if (document.OwnerId != ownerId) return (false, "Access denied");

        var user = await _accountRepository.GetByUserNameAsync(userName);
        if (user == null) return (false, "No such user");
        
        var reviewer = await _documentRepository.GetReviewerAsync(documentId, user.Id);
        if (reviewer == null) return (false, "No such reviewer");
        
        await _commentRepository.DeleteAllReviewerCommentsAsync(reviewer.Id);
        return (true, "Reviewer comments has been deleted");
    }

    public async Task<(bool Success, string Message, List<Guid> Content)> GetAllReviewAssignmentDocumentsAsync(Guid userId)
    {
        var documents = await _documentRepository.GetAllReviewAssignedDocumentsAsync(userId);
        if (documents.Count == 0) return (false, "You don't have any review assignments", []);

        var documentsIds = new List<Guid>();

        foreach (var document in documents)
        {
            var role = await _documentRepository.GetUserRoleAsync(document.Id, userId);
            if (role == "Reviewer") documentsIds.Add(document.Id);
        }
        
        if (documentsIds.Count == 0) return (false, "You don't have any review assignments", []);
        return (true, "Review assignments has been found", documentsIds);
    }
    
    // Too big file overall, splitting things by features would help that
}