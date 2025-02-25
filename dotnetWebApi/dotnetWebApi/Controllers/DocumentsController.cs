using dotnetWebApi.Data;
using Microsoft.AspNetCore.Mvc;
using dotnetWebApi.Entities;

namespace dotnetWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentsController : ControllerBase
{
    private readonly S3DocumentService _s3Service;

    public DocumentsController(S3DocumentService s3Service)
    {
        _s3Service = s3Service;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateDocument([FromBody] Document document)
    {
        if (string.IsNullOrEmpty(document.Body))
            return BadRequest("Документ не может быть пустым.");

        await _s3Service.UploadDocumentAsync(document);
        return Ok("Документ успешно создан.");
    }

    [HttpGet("{title}")]
    public async Task<IActionResult> GetDocumentById(string title)
    {
        var documentBody = await _s3Service.DownloadDocumentAsync(title);
        if (documentBody == null) return NotFound("Документ не найден.");

        return Ok(new { Title = title, Body = documentBody });
    }
    
    [HttpPut("update")]
    public async Task<IActionResult> UpdateDocument([FromBody] Document document)
    {
        if (string.IsNullOrEmpty(document.Body)) 
            return BadRequest("Документ не может быть пустым.");

        try
        {
            await _s3Service.UpdateDocumentAsync(document);
            return Ok("Документ обновлён.");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Документ не найден.");
        }
    }

    [HttpDelete("{title}")]
    public async Task<IActionResult> DeleteDocument(string title)
    {
        await _s3Service.DeleteDocumentAsync(title);
        return Ok("Документ удалён.");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDocuments()
    {
        var documents = await _s3Service.ListDocumentsAsync();
        return Ok(documents);
    }
}