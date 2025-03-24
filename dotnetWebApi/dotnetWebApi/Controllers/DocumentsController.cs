using System.Text;
using dotnetWebApi.Documents.Models;
using dotnetWebApi.Documents.Services;
using dotnetWebApi.Entities;
using Microsoft.AspNetCore.Mvc;
using dotnetWebApi.Extensions;
using dotnetWebApi.Models;
using dotnetWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Minio;
using Minio.DataModel.Args;

namespace dotnetWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly DocumentService _documentService;

    public DocumentsController(DocumentService documentService)
    {
        _documentService = documentService;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequest request)
    {
        var userId = this.GetUserId();
        
        if (string.IsNullOrEmpty(request.Title)) return BadRequest(new {message = "Title is required"});
        await _documentService.CreateDocumentAsync(userId, request.Title, request.Content);
        return Ok("Document is created");
    }

    [HttpGet("{documentId}")]
    public async Task<IActionResult> GetDocument(Guid documentId)
    {
        var userId = this.GetUserId();
        var (success, message, content, role) = await _documentService.GetDocumentAsync(documentId, userId);
        if (!success) return BadRequest(message);
        return Ok(new GetDocumentResponse { Content = content, Role = role});
    }

    [HttpGet("{documentId}/download")]
    public async Task<IActionResult> DownloadDocument(Guid documentId)
    {
        var userId = this.GetUserId();
        
        var (success, message, content) = await _documentService.DownloadDocumentAsync(documentId, userId);
        if (!success) return BadRequest(message);
        var bytes = Encoding.UTF8.GetBytes(content);
        return File(bytes, "text/plain", "document.txt");
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUserDocuments()
    {
        var userId = this.GetUserId();
        
        var documents = await _documentService.GetUserDocumentsAsync(userId);
        
        return Ok(documents);
    }

    [HttpGet("reviewAssignments")]
    public async Task<IActionResult> GetReviewAssignDocuments()
    {
        var userId = this.GetUserId();
        var (success, message, content) = await _documentService.GetAllReviewAssignmentDocumentsAsync(userId);
        if (!success) return BadRequest(message);
        return Ok(content);
    }

    [HttpPut("{documentId}/settings")]
    public async Task<IActionResult> UpdateDocumentSettings(Guid documentId,
        [FromBody] UpdateDocumentSettingsRequest request)
    {
        var userId = this.GetUserId();
        var (success, message) = await _documentService.UpdateDocumentSettingsAsync(documentId, userId, request);
        
        if (!success) return BadRequest(message);
        return Ok("Document settings updated");
    }

    [HttpPut("{documentId}")]
    public async Task<IActionResult> EditDocumentAsync(Guid documentId, [FromBody] EditDocumentRequest request)
    {
        var userId = this.GetUserId();
        
        var (success, message) = await _documentService.EditDocumentAsync(userId, documentId, request.Content);
        
        if (!success) return BadRequest(message);
        
        return Ok(new { message = "Document edited" });
    }

    [HttpDelete("{documentId}")]
    public async Task<IActionResult> DeleteDocument(Guid documentId)
    {
        var userId = this.GetUserId();
        
        var (success, message) = await _documentService.DeleteDocumentAsync(documentId, userId);
        if (!success) return BadRequest(message);
        return Ok("Document deleted");
    }

    [HttpPost("{documentId}/reviewers")]
    public async Task<IActionResult> AddReviewer(Guid documentId, string userName)
    {
        var userId = this.GetUserId();
        
        var (success, addedReviewerUserId, message) = await _documentService.AddReviewerAsync(documentId, userId, userName);
        
        if (!success) return BadRequest(message);

        return Ok($"Added to reviewers user with id: {addedReviewerUserId}");
    }

    [HttpGet("{documentId}/reviewers")]
    public async Task<IActionResult> GetReviewers(Guid documentId)
    {
        var userId = this.GetUserId();
        var (success, message, reviewers) = await _documentService.GetAllReviewersAsync(userId, documentId);
        if (!success) return BadRequest(message);
        return Ok(reviewers);
    }

    [HttpDelete("{documentId}/reviewers/{userName}")]
    public async Task<IActionResult> RemoveReviewer(Guid documentId, string userName)
    {
        var ownerId = this.GetUserId();
        
        var (success, message) = await _documentService.DeleteReviewerAsync(documentId, ownerId, userName);
        if (!success) return BadRequest(message);
        return Ok("Reviewer removed");
    }

    [HttpPost("{documentId}/comments")]
    public async Task<IActionResult> AddComment(Guid documentId, [FromBody] AddCommentRequest request)
    {
        var userId = this.GetUserId();
        
        var (success, message) = await _documentService.AddCommentAsync(documentId, userId, request.Comment);
        if (!success) return BadRequest(message);
        return Ok("Comment added");
    }

    [HttpGet("{documentId}/comments/{commentId}")]
    public async Task<IActionResult> GetComment(Guid commentId)
    {
        var (success, message, content) = await _documentService.GetCommentAsync(commentId);
        if (!success) return BadRequest(message);
        return Ok(content);
    }

    [HttpGet("{documentId}/comments")]
    public async Task<IActionResult> GetAllDocumentComments(Guid documentId)
    {
        var userId = this.GetUserId();
        var (success, message, comments) = await _documentService.GetAllCommentsAsync(userId, documentId);
        if (!success) return BadRequest(message);
        return Ok(comments);
    }

    [HttpDelete("{documentId}/comments/{commentId}")]
    public async Task<IActionResult> RemoveComment(Guid documentId, Guid commentId)
    {
        var userId = this.GetUserId();
        
        var (success, message) = await _documentService.DeleteCommentAsync(documentId, userId, commentId);
        if (!success) return BadRequest(message);
        return Ok("Comment removed");
    }

    [HttpDelete("{documentId}/comments/")]
    public async Task<IActionResult> RemoveAllComments(Guid documentId)
    {
        var ownerId = this.GetUserId();
        var (success, message) = await _documentService.DeleteAllCommentsAsync(documentId, ownerId);
        if (!success) return BadRequest(message);
        return Ok("Comments removed");
    }

    [HttpGet("{documentId}/comments/user/{userName}")]
    public async Task<IActionResult> GetAllReviewerComments(Guid documentId, string userName)
    {
        var userId = this.GetUserId();
        var (success, message, content) = await _documentService.GetAllReviewerCommentsAsync(userId, documentId, userName);
        if (!success) return BadRequest(message);
        return Ok(content);
    }

    [HttpDelete("{documentId}/comments/user/{userName}")]
    public async Task<IActionResult> RemoveAllReviewerComments(Guid documentId, string userName)
    {
        var userId = this.GetUserId();

        var (success, message) = await _documentService.DeleteAllReviewerCommentsAsync(userId, documentId, userName);
        if (!success) return BadRequest(message);
        return Ok("Comments removed");
    }

    [HttpPost("{documentId}/comments/{commentId}/replies")]
    public async Task<IActionResult> AddReply(Guid documentId, Guid commentId, [FromBody] AddCommentRequest request)
    {
        var userId = this.GetUserId();
        var (success, message) = await _documentService.AddReplyToTheCommentAsync(documentId, userId, commentId, request.Comment);
        if (!success) return BadRequest(message);
        return Ok("Reply added");
    }

    [HttpGet("{documentId}/comments/{commentId}/replies")]
    public async Task<IActionResult> GetRepliesToTheComment(Guid documentId, Guid commentId)
    {
        var userId = this.GetUserId();
        var (success, message, replies) = await _documentService.GetAllRepliesAsync(documentId, commentId, userId);
        if (!success) return BadRequest(message);
        return Ok(replies);
    }

    [HttpDelete("{documentId}/comments/{commentId}/replies")]
    public async Task<IActionResult> RemoveReply(Guid documentId, Guid commentId)
    {
        var userId = this.GetUserId();
        var (success, message) = await _documentService.DeleteCommentAsync(documentId, userId, commentId);
        if (!success) return BadRequest(message);
        return Ok("Reply removed");
    }

    [HttpPut("{documentId}/comments")]
    public async Task<IActionResult> UpdateCommentBranchAvailability(Guid documentId, UpdateDocumentSettingsRequest request)
    {
        var userId = this.GetUserId();
        var (success, message) = await _documentService.UpdateDocumentCommentsSettingsAsync(documentId, userId, request);
        if (!success) return BadRequest(message);
        return Ok("Updated");
    }
}