using System.Text;
using dotnetWebApi.Documents.Services;
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
        var (success, message, content, role) = await _documentService.GetDocumentAsync(userId, documentId);
        if (!success) return BadRequest(message);
        return Ok(new GetDocumentResponse { Content = content, Role = role });
    }
}