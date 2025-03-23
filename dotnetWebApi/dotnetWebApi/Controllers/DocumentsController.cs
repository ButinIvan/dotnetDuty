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
    public async Task<IActionResult> AddReviewer(Guid documentId, [FromBody] AddReviewerRequest request)
    {
        var userId = this.GetUserId();
        
        var (success, reviewer, message) = await _documentService.AddReviewerAsync(documentId, userId, request.userName);
        
        if (!success) return BadRequest(message);

        return Ok(reviewer);
    }

    [HttpGet("{documentId}/reviewers")]
    public async Task<IActionResult> GetReviewers(Guid documentId)
    {
        var reviewers = await _documentService.GetAllReviewersAsync(documentId);
        return Ok(reviewers);
    }

    [HttpDelete("{documentId}/reviewers/{userId}")]
    public async Task<IActionResult> RemoveReviewer(Guid documentId, Guid userId)
    {
        var ownerId = this.GetUserId();
        
        var (success, message) = await _documentService.DeleteReviewerAsync(documentId, ownerId, userId);
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

    [HttpGet("{documentId}/comments")]
    public async Task<IActionResult> GetAllDocumentComments(Guid documentId)
    {
        var comments = await _documentService.GetAllCommentsAsync(documentId);
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
}